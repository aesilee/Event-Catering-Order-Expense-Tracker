-- First handle the EventTable
-- Create temporary table with ALL columns from your original table
SELECT * INTO Temp_EventTable FROM EventTable;
DROP TABLE EventTable;

-- Create new EventTable with correct schema (FIXED MenuType and MenuDetails)
CREATE TABLE [dbo].[EventTable] (
    [EventID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [EventTitle] NVARCHAR(100) NOT NULL,
    [EventType] NVARCHAR(500) NOT NULL,
    [EventDate] DATE NOT NULL,
    [EventTime] TIME(7) NOT NULL,
    [Venue] NVARCHAR(100) NOT NULL,
    [CustomerName] NVARCHAR(100) NOT NULL,
    [ContactNumber] NVARCHAR(20) NOT NULL,
    [EmailAddress] NVARCHAR(100) NOT NULL,
    [NumberOfGuests] INT NOT NULL,
    [MenuType] NVARCHAR(50) NOT NULL,
    [MenuDetails] NVARCHAR(MAX) NOT NULL,
    [CustomerNotes] NVARCHAR(MAX) NOT NULL,
    [EstimatedBudget] DECIMAL(18, 2) NOT NULL
);

-- Insert data back WITHOUT EventID (let IDENTITY generate new IDs)
INSERT INTO EventTable (
    [EventTitle],
    [EventType],
    [EventDate],
    [EventTime],
    [Venue],
    [CustomerName],
    [ContactNumber],
    [EmailAddress],
    [NumberOfGuests],
    [MenuType],
    [MenuDetails],
    [CustomerNotes],
    [EstimatedBudget]
)
SELECT 
    [EventTitle],
    [EventType],
    [EventDate],
    [EventTime],
    [Venue],
    [CustomerName],
    [ContactNumber],
    [EmailAddress],
    [NumberOfGuests],
    [MenuType],  -- This maps the old typo column to the correct new column
    [MenuDetails], -- This maps the old typo column to the correct new column
    [CustomerNotes],
    [EstimatedBudget]
FROM Temp_EventTable;

DROP TABLE Temp_EventTable;

-- Now handle ExpensesTable if it exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ExpensesTable')
BEGIN
    -- If ExpensesTable exists, drop and recreate it (since EventIDs will change)
    DROP TABLE ExpensesTable;
END

-- Create fresh ExpensesTable
CREATE TABLE [dbo].[ExpensesTable] (
    [ExpenseID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [EventID] INT NOT NULL,
    [FoodBeverages] DECIMAL(18, 2) NOT NULL,
    [Labor] DECIMAL(18, 2) NOT NULL,
    [Decorations] DECIMAL(18, 2) NOT NULL,
    [Rentals] DECIMAL(18, 2) NOT NULL,
    [Transportation] DECIMAL(18, 2) NOT NULL,
    [Miscellaneous] DECIMAL(18, 2) NOT NULL,
    [TotalExpenses] DECIMAL(18, 2) NOT NULL,
    [BudgetStatus] NVARCHAR(20) NOT NULL,
    FOREIGN KEY ([EventID]) REFERENCES [EventTable]([EventID])
);