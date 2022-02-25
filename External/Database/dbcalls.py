import sqlite3
from sqlite3 import Error

class db:
    def __init__(self):
        self.conn = sqlite3.connect("./src/database/Training_System.db")

    def closeConnection(self):
        self.conn.close()

    def execute_query(self, connection, query):
        cursor = connection.cursor()
        try:
            cursor.execute(query)
            connection.commit()
            print("Query executed successfully")
        except Error as e:
            print(f"The error '{e}' occurred")

    def search_query(self, connection, query):
        cursor = connection.cursor()
        try:
            cursor.execute(query)
            rows = cursor.fetchall()
            if len(rows) < 1:
                return None
            else:
                return rows
            # connection.commit()
        except Error as e:
            print(f"The error '{e}' occurred")


def createTables(self, db):
    query = (
        """
        CREATE TABLE IF NOT EXISTS USERS (
            USER_ID INTEGER PRIMARY KEY,
            USERNAME TEXT NOT NULL,
            PASSWORD TEXT NOT NULL
        )
        """
    )
    db.execute_query(self, db.conn, query)

    query = (
        """
        CREATE TABLE IF NOT EXISTS SCORETYPES(
            SCORETYPE_ID INTEGER PRIMARY KEY,
            NAME TEXT NOT NULL
        )
        """
    )
    db.execute_query(self, db.conn, query)


    query = (
        """
        CREATE TABLE IF NOT EXISTS GAMES(
            GAME_ID INTEGER PRIMARY KEY,
            NAME TEXT NOT NULL,
            EXELINK TEXT NOT NULL
        )
        """
    )
    db.execute_query(self, db.conn, query)


    query = (
        """
        CREATE TABLE IF NOT EXISTS SCORES(
            SCORE_ID INTEGER PRIMARY KEY,
            USER_ID INTEGER,
            SCORETYPE_ID INTEGER,
            GAME_ID INTEGER,
            VALUE INTEGER,
            FOREIGN KEY (USER_ID) 
                REFERENCES USERS (USER_ID) 
                    ON DELETE CASCADE 
                    ON UPDATE NO ACTION,
            FOREIGN KEY (SCORETYPE_ID) 
                REFERENCES SCORETYPES (SCORETYPE_ID) 
                    ON DELETE CASCADE 
                    ON UPDATE NO ACTION, 
            FOREIGN KEY (GAME_ID) 
                REFERENCES GAMES (GAME_ID) 
                    ON DELETE CASCADE 
                    ON UPDATE NO ACTION 
        )
        """
    )
    db.execute_query(self, db.conn, query)



def addUser(db, UserName, Password):
    query = (
        "INSERT INTO USERS(USERNAME, PASSWORD)"
        "VALUES('"+UserName+"','"+Password+"')"
    )
    db.execute_query(db.conn, query)
    return findUserID(db, UserName)

def findUserID(db, UserName):
    query = (
        "SELECT * FROM USERS WHERE USERNAME='"+UserName+"'"
    )
    print(query)
    return db.search_query(db.conn, query)
    

def addScore(db, USER_ID, SCORETYPE_ID, GAME_ID, VALUE):
    query = (
        "INSERT INTO SCORES(USER_ID, SCORETYPE_ID, GAME_ID, VALUE)"
        "VALUES("+USER_ID+","+SCORETYPE_ID+","+GAME_ID+","+VALUE+")"
    )
    db.execute_query(db.conn, query)

