CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO


CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [Country] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [User] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Username] ON [User] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_UserGuid] ON [User] ([UserGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [PollAnswer] ([PollId] ASC)
GO
 

CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [ArticleCategory] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [ArticleCategory] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [Forums_Group] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [Forums_Forum] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [Forums_Forum] ([ForumGroupId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [Forums_Topic] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [Forums_Post] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_UserId] ON [Forums_Post] ([UserId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
GO
 