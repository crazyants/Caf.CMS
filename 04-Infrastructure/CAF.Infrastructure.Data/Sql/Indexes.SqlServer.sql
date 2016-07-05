CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])
GO

CREATE NONCLUSTERED INDEX [IX_LocalizedProperty_Key] ON [LocalizedProperty] ([Id])	INCLUDE ([EntityId], [LocaleKeyGroup], [LocaleKey])
GO