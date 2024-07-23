"""

import pymssql
conn = pymssql.connect(server='120.138.8.94, 14433', user='k4m2a',
                       password='D7gefr98F&1lbIjec', database='k4m2a')

# import pymssql
# conn = pymssql.connect(server='yourserver.database.windows.net', user='yourusername@yourserver', password='yourpassword', database='AdventureWorks')
cursor = conn.cursor()
cursor.execute('SELECT c.CustomerID, c.CompanyName,COUNT(soh.SalesOrderID) AS OrderCount FROM SalesLT.Customer AS c LEFT OUTER JOIN SalesLT.SalesOrderHeader AS soh ON c.CustomerID = soh.CustomerID GROUP BY c.CustomerID, c.CompanyName ORDER BY OrderCount DESC;')
row = cursor.fetchone()
while row:
    print(str(row[0]) + " " + str(row[1]) + " " + str(row[2]))
    row = cursor.fetchone()

    


import pyodbc

# Define the connection parameters
driver = 'ODBC Driver 18 for SQL Server'
server = '120.138.8.94'
# server = '120.138.8.94,14433'
port = '14433'
database = 'k4m2a'
username = 'k4m2a'
password = 'D7gefr98F&1lbIjec'

# Create a connection string
connection_string = f'DRIVER={driver};SERVER={server};DATABASE={database};UID={username};PWD={password}'


# Establish the connection
connection = pyodbc.connect(connection_string)

# Create a cursor object to interact with the database
cursor = connection.cursor()

# # Example: Execute a simple query
# cursor.execute('SELECT * FROM your_table_name')
# # Fetch the results
# rows = cursor.fetchall()

# # Example: Print the results
# for row in rows:
#     print(row)

# # Close the cursor and connection
# cursor.close()
# connection.close()

"""
from pprint import pprint, pformat
import pyodbc

# Define the connection parameters
# driver = '/opt/homebrew/lib/libmsodbcsql.18.dylib'
driver = 'ODBC Driver 17 for SQL Server'
# server = '120.138.8.94'
server = '120.138.8.94, 14433'
port = '14433'
database = 'k4m2a'
username = 'k4m2a'
password = 'D7gefr98F&1lbIjec'

db_connection = pyodbc.connect(
    driver=driver,
    server=server,
    # port=port,
    database=database,
    UID=username,
    password=password
)

# Create a cursor object to interact with the database
cursor = db_connection.cursor()

# Example: Execute a simple query
cursor.execute('SELECT * FROM dbo.Reaction')
# Fetch the results
rows = cursor.fetchall()

# Example: Print the results
for row in rows:
    print(pformat(row))

# Close the cursor and connection
cursor.close()
db_connection.close()
