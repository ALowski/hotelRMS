Matlab Runtime is employed to execute a compiled MATLAB application. Basic library of application is “quadprog”. It runs optimization model and calculate prices and occupancies. A hotel shall have Matlab Runtime installed on their PC.

To run a software a hotel shall enter parameters related to its activity and input data. Pictures 1 - 3 show user interface of input of parameters.

Input data shall be structured as follows. First column is - date of a booking, 2nd - date of arrival, 3rd - length of stay, 4th - price of a room, 5th - room type, 6th - meal type. Example of input data is in the file "Input data example.csv". 

Example of output results is in the file "Output data example_12.08.2011.csv". 
It contains prices and occupancies from start day to end of the planning horizon. Names of columns describe demand categories. First number tells about room type. Second number tells about lengths of stay and booking window. Third number tells about meal type. In this particular case second number means as follows. 
1 - length of stay is between 1 and 7 days, booking window is between 0 and 7 days, 
2 - length of stay is between 1 and 7 days, booking window is between 8 and 30 days,
3 - length of stay is between 1 and 7 days, booking window is more than 30 days,
4 - length of stay is between 8 and 30 days, booking window is between 0 and 7 days,
5 - length of stay is between 8 and 30 days, booking window is between 8 and 30 days,
6 - length of stay is between 8 and 30 days, booking window is more than 30 days,
7 - length of stay is more than 30 days, booking window is between 0 and 7 days,
8 - length of stay is more than 30 days, booking window is between 8 and 30 days,
9 - length of stay is more than 30 days, booking window is more than 30 days.  
