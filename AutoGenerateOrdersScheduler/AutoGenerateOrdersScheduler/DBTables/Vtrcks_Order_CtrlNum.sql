/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [VtrcksOrderID]
      ,[Ctrl_Num]
  FROM [jatindb].[dbo].[Vtrcks_Order_CtrlNum]

  -- update Vtrcks_Order_CtrlNum set Ctrl_Num = 15231 where VtrcksOrderID = 'OrderID'

  -- insert into [dbo].[Vtrcks_Order_CtrlNum] values ('retwast',104251)