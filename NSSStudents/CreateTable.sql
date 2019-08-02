USE MASTER
GO;

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases-- Get a list of databases
    WHERE [name] = N'StudentsExercises'
)
CREATE DATABASE StudentsExercises
GO;

USE StudentsExercises
GO;


CREATE TABLE Cohort (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    CohortName VARCHAR(50) NOT NULL,
);


CREATE TABLE Exercise (
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    ExerciseName VARCHAR(50) NOT NULL,
    ExerciseLanguage VARCHAR(50) NOT NULL
);

CREATE TABLE Student (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    SlackHandle VARCHAR(50) NOT NULL,
    CohortId INT NOT NULL,
    CONSTRAINT FK_Student_Cohort FOREIGN KEY (CohortId) REFERENCES Cohort(Id)
);

CREATE TABLE StudentExercise ( 
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    StudentId INTEGER NOT NULL,
    ExerciseId INTEGER NOT NULL,
    CONSTRAINT FK_Student FOREIGN KEY (StudentId) REFERENCES Student(Id),
    CONSTRAINT FK_Exercise FOREIGN KEY (ExerciseId) REFERENCES Exercise(Id)
);

CREATE TABLE Instructor (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    SlackHandle VARCHAR(50) NOT NULL,
    Specialty VARCHAR(50) NOT NULL,
    CohortId INT NOT NULL,
    CONSTRAINT FK_Instructor_Cohort FOREIGN KEY (CohortId) REFERENCES Cohort(Id)
);