USE [SQLDatabaseForRunners]
GO

ALTER TABLE [dbo].[MapPointItems] DROP CONSTRAINT [FK_dbo.MapPointItems_dbo.WorkoutItems_WorkoutItemId]
GO

ALTER TABLE [dbo].[MapPointItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MapPointItems_dbo.WorkoutItems_WorkoutItemId] FOREIGN KEY([WorkoutItemId])
REFERENCES [dbo].[WorkoutItems] ([Id])  ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MapPointItems] CHECK CONSTRAINT [FK_dbo.MapPointItems_dbo.WorkoutItems_WorkoutItemId]
GO


USE [SQLDatabaseForRunners]
GO

ALTER TABLE [dbo].[WorkoutItems] DROP CONSTRAINT [FK_dbo.WorkoutItems_dbo.UserItems_UserItemId]
GO

ALTER TABLE [dbo].[WorkoutItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WorkoutItems_dbo.UserItems_UserItemId] FOREIGN KEY([UserItemId])
REFERENCES [dbo].[UserItems] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WorkoutItems] CHECK CONSTRAINT [FK_dbo.WorkoutItems_dbo.UserItems_UserItemId]
GO



