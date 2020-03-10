import sqlite3



conn = sqlite3.connect('TowerDefenseDB.db')
c = conn.cursor()

c = conn.cursor()

# Create table
#c.execute(""" CREATE TABLE users (address text, login text, password text) """)
#insert values
c.execute(""" INSERT INTO users VALUES ('0x456', 'szymix', '0x2dbc') """)
#delete
#c.execute( """ DELETE FROM users WHERE login='szymix' """)
#print all 
c.execute( """SELECT * FROM users""" )
print(c.fetchall())

conn.commit()
c.close()