USE MASTER
GO;

USE StudentsExercises
GO;

INSERT INTO Cohort (CohortName) VALUES('Cohort 30');
INSERT INTO Cohort (CohortName) VALUES('Cohort 31');
INSERT INTO Cohort (CohortName) VALUES('Cohort 32');
INSERT INTO Cohort (CohortName) VALUES('Cohort 33');
INSERT INTO Cohort (CohortName) VALUES('Cohort 34');

INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId) VALUES ('Adam', 'Scheaffer', 'adamscheaf', 'Gifs against Humanity', 3);
INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId) VALUES ('Jisie', 'David', 'jisie', 'Amazing NASA shirts', 3);
INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId) VALUES ('Kristen', 'Norris', 'kristen.norris', 'Not blonde', 3);

INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId) VALUES ('Joel', 'Venable', 'joel venable', 3);
INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId) VALUES ('Sean', 'Glavin', 'sean glavin', 3);
INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId) VALUES ('Nate', 'Fleming', 'nate.fleming', 3);
INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId) VALUES ('Emily', 'Loggins', 'emily loggins', 3);


INSERT INTO Exercise (ExerciseName, ExerciseLanguage) VALUES ('Terniary Traveler', 'Javascript');
INSERT INTO Exercise (ExerciseName, ExerciseLanguage) VALUES ('Trestlebridge Farms', 'C Sharp');
INSERT INTO Exercise (ExerciseName, ExerciseLanguage) VALUES ('Students Exercise', 'C Sharp');
INSERT INTO Exercise (ExerciseName, ExerciseLanguage) VALUES ('Reactive Nutshell', 'Javascript');
INSERT INTO Exercise (ExerciseName, ExerciseLanguage) VALUES ('Welcome to Nashville', 'Javascript');














