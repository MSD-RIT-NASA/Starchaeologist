import sqlite3
from sqlite3 import Error

class db:
    def __init__(self):
        self.conn = sqlite3.connect("./External/Python/database/Training_System.db")

    def closeConnection(self):
        self.conn.close()

    def execute_query(self, query):
        connection = self.conn
        cursor = connection.cursor()
        try:
            cursor.execute(query)
            connection.commit()
            print("Query executed successfully")
        except Error as e:
            print(f"The error '{e}' occurred")

    def search_query(self, query):
        connection = self.conn
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


    def createTables(self):
        query = (
            """
            CREATE TABLE IF NOT EXISTS USERS (
                USER_ID INTEGER PRIMARY KEY,
                USERNAME TEXT NOT NULL,
                PASSWORD TEXT NOT NULL
            )
            """
        )
        self.execute_query(query)

        query = (
            """
            CREATE TABLE IF NOT EXISTS SCORETYPES(
                SCORETYPE_ID INTEGER PRIMARY KEY,
                NAME TEXT NOT NULL
            )
            """
        )
        self.execute_query(query)


        query = (
            """
            CREATE TABLE IF NOT EXISTS GAMES(
                GAME_ID INTEGER PRIMARY KEY,
                NAME TEXT NOT NULL,
                EXELINK TEXT NOT NULL
            )
            """
        )
        self.execute_query(query)


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
        self.execute_query(query)



    def addUser(self, UserName, Password):
        query = (
            "INSERT INTO USERS(USERNAME, PASSWORD)"
            "VALUES('"+UserName+"','"+Password+"')"
        )
        self.execute_query(query)
        return self.findUserID(UserName)

    def getGameScore(self, USER_ID):
        query = (
            "SELECT VALUE FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='2'"
        )
        return self.search_query(query)

    def getBalanceScore(self, USER_ID):
        query = (
            "SELECT VALUE FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='1'"
        )
        return self.search_query(query)

    def findUserID(self, UserName):
        query = (
            "SELECT * FROM USERS WHERE USERNAME='"+UserName+"'"
        )
        return self.search_query(query)
        

    def addScore(self, USER_ID, SCORETYPE_ID, GAME_ID, VALUE):
        query = (
            "INSERT INTO SCORES(USER_ID, SCORETYPE_ID, GAME_ID, VALUE)"
            "VALUES("+USER_ID+","+SCORETYPE_ID+","+GAME_ID+","+VALUE+")"
        )
        self.execute_query(query)

if __name__ == '__main__':
    database = db()
    query = (
            "INSERT INTO SCORETYPES(NAME)" +
            "VALUES('GAME')"
        )
    database.execute_query(query)