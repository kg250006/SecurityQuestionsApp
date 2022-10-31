using DBService.DB;
using DBService.DB.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;

namespace SecurityQuestionsApp
{
    class Program
    {
        public NLog.Logger _log = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        // The Repository Access
        private readonly SQLConfigRepository _configDB = new SQLConfigRepository(new IContextRepository());
        private List<string> _QuestionList = new List<string>();


        static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

           
            try
            {
                Program prog = new Program();
                
                logger.Debug($" ########### App Started.");
                prog.InitDB();
                var result = 0;
                do
                {
                    // IF result = 0; // End on Failure/Error
                    // IF result = 1; // End on Success
                    // IF result = 2; // Loop process again

                    result = prog.PromptForName();

                } while (result == 2);

                if (result == 0) {
                    logger.Error($"Process Complete on Failure.");
                }
                else if (result == 1)
                {
                    logger.Info($"Process Complete on success.");
                }
                    
                logger.Debug($" <<<<<<<<<<< App End.");

            }
            catch (Exception ex)
            {
                logger.Error($"Stopped program because of exception, ERROR: {ex.Message}");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }


        public Program()
        {
            // load Questions
            _QuestionList.Add("In what city were you born?");
            _QuestionList.Add("What is the name of your favorite pet?");
            _QuestionList.Add("What is your mother's maiden name?");
            _QuestionList.Add("What high school did you attend?");
            _QuestionList.Add("What was the mascot of your high school?");
            _QuestionList.Add("What was the make of your first car?");
            _QuestionList.Add("What was your favorite toy as a child?");
            _QuestionList.Add("Where did you meet your spouse?");
            _QuestionList.Add("What is your favorite meal?");
            _QuestionList.Add("Who is your favorite actor / actress?");

        }

        enum RetVal
        {
            END_FAILURE = 0,
            END_SUCCESS = 1,
            STARTOVER = 2
        };
        public int PromptForName()
        {
            string sFunc = "PromptForName - ";

            _log.Info(sFunc + $"ENTERED Process");

            int retVal = (int)RetVal.END_FAILURE;
            try
            {
                // ask for name
                Console.Clear();
                Console.WriteLine("#############################");
                Console.WriteLine("### SECURITY QUESTION APP ###");
                Console.WriteLine("##### by Kenrick Goldson ####");

                Console.WriteLine("\nHi, what is your name?");
                var name = Console.ReadLine().ToLower();

                _log.Info(sFunc + $"User Name Entry: {name}");

                // does user already exist?
                var nameTemp = _configDB.GetValue($"User.{name}");

                if(string.IsNullOrWhiteSpace(nameTemp))
                {
                    // user does NOT already exist?
                    _log.Info(sFunc + $"User does NOT exist: {name}");
                    Console.WriteLine($"Hello {name}, looks like you are New here.");

                    return processStoreFlow(name);

                }
                else
                {
                    // user already exist?
                    _log.Info(sFunc + $"User already exist: {name}");
                    Console.WriteLine($"Hello {name}, Welcome Back!");

                    // Does User have Security Questions/Answers?
                    var sSecQuestion1 = _configDB.GetValue($"User.{name}.Q1.Question");
                    var sSecQuestion2 = _configDB.GetValue($"User.{name}.Q2.Question");
                    var sSecQuestion3 = _configDB.GetValue($"User.{name}.Q3.Question");

                    var sSecAnswer1 = _configDB.GetValue($"User.{name}.Q1.Answer");
                    var sSecAnswer2 = _configDB.GetValue($"User.{name}.Q2.Answer");
                    var sSecAnswer3 = _configDB.GetValue($"User.{name}.Q3.Answer");

                    if (!string.IsNullOrWhiteSpace(sSecQuestion1) || !string.IsNullOrWhiteSpace(sSecQuestion2) || !string.IsNullOrWhiteSpace(sSecQuestion3)) {
                        Console.WriteLine($"Do you want to answer a security question? (Y/N)");
                        _log.Info(sFunc + $"Question presented to user: 'Do you want to answer a security question'");
                        var bAnswer = Console.ReadLine();
                        if (bAnswer.ToLower() == "y") {
                            _log.Info(sFunc + $"User Answered YES");
                            return processAnswerFlow(name);
                        }
                        else if (bAnswer.ToLower() == "n") {

                            _log.Info(sFunc + $"User Answered NO");
                            return processStoreFlow(name);
                        }
                        else {
                            Console.WriteLine($"ERR: Invalid Response!");
                            Console.WriteLine($"Press ANY KEY to continue.");
                            Console.ReadLine();
                            return (int)RetVal.STARTOVER;
                        }
                    }
                    else
                    {
                        _log.Info(sFunc + $"User({name}) has No Stored Questions");
                        return processStoreFlow(name);
                    }

                }

            }
            catch (Exception ex)
            {
                _log.Error($"Failure during StartApp, ERROR: {ex.Message}");
            }


            return retVal;

        }

        public int processStoreFlow(string name)
        {
            string sFunc = "processStoreFlow - ";
            _log.Info(sFunc + $"ENTERED Process");

            Console.WriteLine($"Would you like to store answers to security questions? (Y/N)");
            _log.Info(sFunc + $"Posing Question: 'Would you like to store answers to security questions?'");
            var bAnswer = Console.ReadLine();
            if (bAnswer.ToLower() == "y")
            {
                _log.Info(sFunc + $"User Answered YES");

                Console.WriteLine($"===============================");
                Console.WriteLine($"Choose Security Questions...");

                int count = 0;
                foreach (var question in _QuestionList) {
                    count++;
                    Console.WriteLine($"{count}. {question}");

                }
                Console.WriteLine($"===============================\n\n");


                Console.WriteLine($"(#1) Choose Question to Answer [1 - {_QuestionList.Count}]:");
                var bQ1choice = Console.ReadLine();
                if (int.TryParse(bQ1choice, out int iQ1choice))
                {
                    Console.WriteLine($"{_QuestionList[iQ1choice - 1]}\nYour Answer:");
                    var bQ1Answer = Console.ReadLine();

                    // Store Question/Answer
                    _configDB.Add(new Config { Name = $"User.{name}.Q1.Question", Value = _QuestionList[iQ1choice-1] });
                    _configDB.Add(new Config { Name = $"User.{name}.Q1.Answer",   Value = bQ1Answer.ToLower() });

                }
                else {
                    _log.Error(sFunc + $"Q1 Invalid Input");
                    Console.WriteLine($"Q1 Invalid Input!");
                    return (int)RetVal.STARTOVER;
                }

                // ================================================
                Console.WriteLine($"----------------------------");
                Console.WriteLine($"(#2) Choose Question to Answer [1 - {_QuestionList.Count}]:");
                var bQ2choice = Console.ReadLine();
                if (int.TryParse(bQ2choice, out int iQ2choice))
                {
                    // make sure question not already used
                    if (iQ1choice == iQ2choice) {
                        Console.WriteLine($"Question Already Used, Choose Again:");

                        bQ2choice = Console.ReadLine();
                        if (int.TryParse(bQ2choice, out iQ2choice))
                        {
                            if (iQ1choice == iQ2choice)
                                return (int)RetVal.STARTOVER;
                        }
                    }

                    Console.WriteLine($"{_QuestionList[iQ2choice - 1]}\nYour Answer:");
                    var bQ2Answer = Console.ReadLine();

                    // Store Question/Answer
                    _configDB.Add(new Config { Name = $"User.{name}.Q2.Question", Value = _QuestionList[iQ2choice - 1] });
                    _configDB.Add(new Config { Name = $"User.{name}.Q2.Answer", Value = bQ2Answer.ToLower() });

                }
                else
                {
                    _log.Error(sFunc + $"Q2 Invalid Input");
                    Console.WriteLine($"Q2 Invalid Input!");
                    Console.WriteLine($"Press ANY KEY to continue.");
                    Console.ReadLine();
                    return (int)RetVal.STARTOVER;
                }

                // ================================================
                Console.WriteLine($"----------------------------");
                Console.WriteLine($"(#3) Choose Question to Answer [1 - {_QuestionList.Count}]:");
                var bQ3choice = Console.ReadLine();
                if (int.TryParse(bQ3choice, out int iQ3choice))
                {
                    // make sure question not already used
                    if (iQ1choice == iQ3choice || iQ2choice == iQ3choice)
                    {
                        Console.WriteLine($"Question Already Used, Choose Again:");

                        bQ3choice = Console.ReadLine();
                        if (int.TryParse(bQ3choice, out iQ3choice))
                        {
                            if (iQ1choice == iQ3choice || iQ2choice == iQ3choice)
                                return (int)RetVal.STARTOVER;
                        }
                    }


                    Console.WriteLine($"{_QuestionList[iQ3choice - 1]}\nYour Answer:");
                    var bQ3Answer = Console.ReadLine();

                    // Store Question/Answer
                    _configDB.Add(new Config { Name = $"User.{name}.Q3.Question", Value = _QuestionList[iQ3choice - 1] });
                    _configDB.Add(new Config { Name = $"User.{name}.Q3.Answer", Value = bQ3Answer.ToLower() });


                }
                else
                {
                    _log.Error(sFunc + $"Q3 Invalid Input");
                    Console.WriteLine($"Q3 Invalid Input!");
                    Console.WriteLine($"Press ANY KEY to continue.");
                    Console.ReadLine();
                    return (int)RetVal.STARTOVER;
                }

                _configDB.Add(new Config { Name = $"User.{name}", Value = name });

            }
            else if (bAnswer.ToLower() == "n")
            {
                _log.Info(sFunc + $"User Answered NO");
            }
            else
            {
                Console.WriteLine($"ERR: Invalid Response!");
                Console.WriteLine($"Press ANY KEY to continue.");
                Console.ReadLine();
                return (int)RetVal.STARTOVER;
            }

            Console.WriteLine($"----------------------------");
            return (int)RetVal.STARTOVER;
        }


        public int processAnswerFlow(string name)
        {
            string sFunc = "processAnswerFlow - ";
            _log.Info(sFunc + $"ENTERED Process");


            // Does User have Security Questions/Answers?
            var sSecQuestion1 = _configDB.GetValue($"User.{name}.Q1.Question");
            var sSecQuestion2 = _configDB.GetValue($"User.{name}.Q2.Question");
            var sSecQuestion3 = _configDB.GetValue($"User.{name}.Q3.Question");

            var sSecAnswer1 = _configDB.GetValue($"User.{name}.Q1.Answer");
            var sSecAnswer2 = _configDB.GetValue($"User.{name}.Q2.Answer");
            var sSecAnswer3 = _configDB.GetValue($"User.{name}.Q3.Answer");

            _log.Info(sFunc + $"Presenting Security Questions to User ({name})");

            if (!string.IsNullOrWhiteSpace(sSecQuestion1))
            {
                Console.WriteLine($"----------------------------");
                Console.WriteLine($"Question: {sSecQuestion1}");
                _log.Info(sFunc + $"Present Q1 to User: {sSecQuestion1}");

                var bQ1Answer = Console.ReadLine();
                _log.Info(sFunc + $"Q1 User Answer: {bQ1Answer}");


                if (bQ1Answer.ToLower() == sSecAnswer1) 
                {
                    _log.Info(sFunc + $"Answer to Q1 is CORRECT! '{bQ1Answer}'");
                    Console.WriteLine($"Congrats!!... Your Answer is CORRECT!");
                    return (int)RetVal.STARTOVER;
                }
                else
                {
                    _log.Info(sFunc + $"Answer to Q1 is NOT CORRECT! '{bQ1Answer}'");
                    Console.WriteLine($"Your Answer is NOT CORRECT!");
                }

            }

            if (!string.IsNullOrWhiteSpace(sSecQuestion2))
            {
                Console.WriteLine($"----------------------------");
                Console.WriteLine($"Question: {sSecQuestion2}");
                _log.Info(sFunc + $"Present Q2 to User: {sSecQuestion2}");

                var bQ2Answer = Console.ReadLine();
                _log.Info(sFunc + $"Q2 User Answer: {bQ2Answer}");

                if (bQ2Answer.ToLower() == sSecAnswer2)
                {
                    _log.Info(sFunc + $"Answer to Q2 is CORRECT! '{bQ2Answer}'");
                    Console.WriteLine($"Congrats!!... Your Answer is CORRECT!");
                    return (int)RetVal.STARTOVER;
                }
                else
                {
                    _log.Info(sFunc + $"Answer to Q2 is NOT CORRECT! '{bQ2Answer}'");
                    Console.WriteLine($"Your Answer is NOT CORRECT!");
                }

            }

            if (!string.IsNullOrWhiteSpace(sSecQuestion3))
            {
                Console.WriteLine($"----------------------------");
                Console.WriteLine($"Question: {sSecQuestion3}");
                _log.Info(sFunc + $"Present Q3 to User: {sSecQuestion3}");

                var bQ3Answer = Console.ReadLine();
                _log.Info(sFunc + $"Q3 User Answer: {bQ3Answer}");

                if (bQ3Answer.ToLower() == sSecAnswer3)
                {
                    _log.Info(sFunc + $"Answer to Q3 is CORRECT! '{bQ3Answer}'");
                    Console.WriteLine($"Congrats!!... Your Answer is CORRECT!");
                    return (int)RetVal.STARTOVER;
                }
                else
                {
                    _log.Info(sFunc + $"Answer to Q3 is NOT CORRECT! '{bQ3Answer}'");
                    Console.WriteLine($"Your Answer is NOT CORRECT!");
                }

            }


            Console.WriteLine($"\n\nSORRY, You have run out of Questions to Answer. :(");
            Console.WriteLine($"Press ANY KEY to continue.");
            Console.ReadLine();

            Console.WriteLine($"----------------------------");

            return (int)RetVal.STARTOVER;
        }


        public bool InitDB()
        {
            string sFunc = "InitDB - ";

            _log.Info(sFunc + "Initializing DB...");
            try
            {
                var dbContext = new IContextRepository();
                dbContext.Database.EnsureCreatedAsync();

                _configDB.Add(new Config() { Name = "VersionName", Value = "1.0" });

                _log.Info(sFunc + "DB Initialized.");
            }
            catch (Exception ex)
            {
                _log.Error(sFunc + $"ERROR: {ex}");
                return false;
            }

            return true;
        }
    }
}
