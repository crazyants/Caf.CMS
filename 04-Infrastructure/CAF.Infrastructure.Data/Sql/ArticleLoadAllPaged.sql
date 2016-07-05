ALTER PROCEDURE [dbo].[ArticleLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@SiteId			int = 0,
	@ArticleTagId		int = 0,
	@FeaturedArticles	bit = null,	--0 featured only , 1 not featured only, null - load all Articles
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in Article descriptions
	@SearchArticleTags  bit = 0, --a value indicating whether to search by a specified "keyword" in Article tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedUserRoleIds	nvarchar(MAX) = null,	--a list of User role IDs (comma-separated list) for which a Article should be shown (if a subjet to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@IsHot				bit = null,
	@IsRed				bit = null,
	@IsTop				bit = null,
	@IsSlide			bit = null,
	@WithoutCategories	bit = 0,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	/* Articles that filtered by keywords */
	CREATE TABLE #KeywordArticles
	(
		[ArticleId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, LEN(@Keywords) - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, LEN(@Keywords)) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--Article name
		SET @sql = '
		INSERT INTO #KeywordArticles ([ArticleId])
		SELECT p.Id
		FROM Article p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Title], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Title]) > 0 '


		--localized Article name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Article''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
	

		IF @SearchDescriptions = 1
		BEGIN
			--Article short description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Article p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[ShortContent], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[ShortContent]) > 0 '


			--Article full description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Article p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[FullContent], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[FullContent]) > 0 '



			--localized Article short description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Article''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''ShortContent'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
				

			--localized Article full description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Article''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''FullContent'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

	

		IF @SearchArticleTags = 1
		BEGIN
			--Article tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Article_Id
			FROM Article_ArticleTag_Mapping pptm with(NOLOCK) INNER JOIN ArticleTag pt with(NOLOCK) ON pt.Id = pptm.ArticleTag_Id
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(pt.[Name], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, pt.[Name]) > 0 '

			--localized Article tag
			SET @sql = @sql + '
			UNION
			SELECT pptm.Article_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Article_ArticleTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ArticleTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ArticleTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000)', @Keywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [caf_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)


	--filter by User role IDs (access control list)
	SET @AllowedUserRoleIds = isnull(@AllowedUserRoleIds, '')	
	CREATE TABLE #FilteredUserRoleIds
	(
		UserRoleId int not null
	)
	INSERT INTO #FilteredUserRoleIds (UserRoleId)
	SELECT CAST(data as int) FROM [caf_splitstring_to_table](@AllowedUserRoleIds, ',')
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)

	SET @sql = '
	INSERT INTO #DisplayOrderTmp ([ArticleId])
	SELECT p.Id
	FROM
		Article p with (NOLOCK)'

	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN ArticleCategory pcm with (NOLOCK)
			ON p.CategoryId = pcm.Id'
	END
	
	IF ISNULL(@ArticleTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Article_ArticleTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Article_Id'
	END
		
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordArticles kp
			ON  p.Id = kp.ArticleId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
		--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.Id IN (SELECT CategoryId FROM #FilteredCategoryIds)'
		
	END
	--filter by Article tag
	IF ISNULL(@ArticleTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ArticleTag_Id = ' + CAST(@ArticleTagId AS nvarchar(max))
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.StatusId = 0
		AND p.Deleted = 0
		 '
	END
	--Sho EnableValidation
	IF @IsHot IS NOT NULL
	BEGIN
		SET @sql = @sql + '
		AND p.IsHot = ' + CAST(@IsHot AS nvarchar(max))
	END
	IF @IsRed IS NOT NULL
	BEGIN
		SET @sql = @sql + '
		AND p.IsRed = ' + CAST(@IsRed AS nvarchar(max))
	END
	IF @IsTop IS NOT NULL
	BEGIN
		SET @sql = @sql + '
		AND p.IsTop = ' + CAST(@IsTop AS nvarchar(max))
	END
	IF @IsSlide IS NOT NULL
	BEGIN
		SET @sql = @sql + '
		AND p.IsSlide = ' + CAST(@IsSlide AS nvarchar(max))
	END
	
	--show hidden and ACL
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredUserRoleIds [fcr]
			WHERE
				[fcr].UserRoleId IN (
					SELECT [acl].UserRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Article''
				)
			))'
	END
	
	--show hidden and filter by Site
	IF @SiteId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToSites = 0 OR EXISTS (
			SELECT 1 FROM [SiteMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Article'' and [sm].SiteId=' + CAST(@SiteId AS nvarchar(max)) + '
			))'
	END
	
	
	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Title] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Title] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.DisplayOrder ASC,p.[Title] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
	PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredUserRoleIds
	DROP TABLE #KeywordArticles

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ArticleId])
	SELECT ArticleId
	FROM #DisplayOrderTmp
	GROUP BY ArticleId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	

	--return Articles
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Article p with (NOLOCK) on p.Id = [pi].[ArticleId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END