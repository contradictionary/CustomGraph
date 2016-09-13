# CustomGraph
A WPFtoolkit Chart implemented for monitoring the Data processed within a Specified time interval by Taking a CSV file as input



Overwiew : 
1- Its quite smple and deasy implemetation of the WPFToolkit Charting Cotrol.
2- Takes the input from CSV file into Datamodel Class named APISTATDataModel, Its Designed purely based upon the CSV file.So incase you are thinking of using it you have to modify APISTATDataModel & GraphDataModel.cs class according to your CSV file that you are going to use with it.
3- The graph  Refreshes after the  a specific Time interval to update the data from CSV ( As the CSV file updateds every 10 minute - in my case ).
