# SecurityQuestionsApp
by Kenrick Goldson

App to show use case process of handling user security questions


## Overview

The intent was to ensure that the flow was broken down among individual methods to effectively organize the process flow. Each process returns an integer to allow for more lucrative return actions. 

- public int PromptForName()
- public int processStoreFlow(string name)
- public int processAnswerFlow(string name)
- InitDB()

This application uses NLog to allow monitoring transactions and the archiving of logs.



## Storage

The persistent storage used was SQLlite built using DBContext from Microsoft.EntityFrameworkCore. This allows for the on the spot creation of the database structure, using the code 1st approach without the need of any complex external resources.

The structure of the data stores user questions and information in a dot notation format in a single table.
