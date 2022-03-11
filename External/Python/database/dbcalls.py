import sqlite3
from sqlite3 import Error

class db:
    def __init__(self):
        self.conn = sqlite3.connect("./External/Python/database/Training_System.db",check_same_thread=False)

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
            "SELECT VALUE, DATE_ADDED FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='2' ORDER BY DATE_ADDED ASC"
        )
        return self.search_query(query)

    def getBalanceScore(self, USER_ID):
        query = (
            "SELECT VALUE, DATE_ADDED FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='1' ORDER BY DATE_ADDED ASC"
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

    def getTopScores(self):
        queryOne = """
        SELECT VALUE, USERS.USERNAME FROM SCORES INNER JOIN USERS on USERS.USER_ID = SCORES.USER_ID
        WHERE GAME_ID = 1 AND SCORETYPE_ID = 2
        ORDER BY VALUE DESC
        LIMIT 10
        """
        gameScoreOne = self.search_query(queryOne)
        queryTwo = """
        SELECT VALUE, USERS.USERNAME FROM SCORES INNER JOIN USERS on USERS.USER_ID = SCORES.USER_ID
        WHERE GAME_ID = 2 AND SCORETYPE_ID = 2
        ORDER BY VALUE DESC
        LIMIT 10
        """
        gameScoreTwo = self.search_query(queryTwo)
        queryThree = """
        SELECT VALUE, USERS.USERNAME FROM SCORES INNER JOIN USERS on USERS.USER_ID = SCORES.USER_ID
        WHERE GAME_ID = 3 AND SCORETYPE_ID = 2
        ORDER BY VALUE DESC
        LIMIT 10
        """
        gameScoreThree = self.search_query(queryThree)
        return gameScoreOne, gameScoreTwo, gameScoreThree

if __name__ == '__main__':
    database = db()
    # database.getTopScores()
    # query = """
    #     CREATE TABLE IF NOT EXISTS SCORES(
    #             SCORE_ID INTEGER PRIMARY KEY,
    #             USER_ID INTEGER,
    #             SCORETYPE_ID INTEGER,
    #             GAME_ID INTEGER,
    #             VALUE INTEGER,
    #             DATE_ADDED TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    #             FOREIGN KEY (USER_ID) 
    #                 REFERENCES USERS (USER_ID) 
    #                     ON DELETE CASCADE 
    #                     ON UPDATE NO ACTION,
    #             FOREIGN KEY (SCORETYPE_ID) 
    #                 REFERENCES SCORETYPES (SCORETYPE_ID) 
    #                     ON DELETE CASCADE 
    #                     ON UPDATE NO ACTION, 
    #             FOREIGN KEY (GAME_ID) 
    #                 REFERENCES GAMES (GAME_ID) 
    #                     ON DELETE CASCADE 
    #                     ON UPDATE NO ACTION 
    #         )
    # """
    # database.execute_query(query)
    # query = """
    #             INSERT INTO SCORES (USER_ID, SCORETYPE_ID, GAME_ID, VALUE)
    #             SELECT USER_ID, SCORETYPE_ID, GAME_ID, VALUE 
    #             FROM SCORES_OLD 
    #         """
    # database.execute_query(query)
    
    # query = """
    #             DROP TABLE SCORES_OLD 
    #         """
   
    # query = """
    # INSERT INTO GAMES(NAME, EXELINK)
    # VALUES("River Rafting", ""),
    # ("Searching", ""),
    # ("Mystery", "")
    # """
    query = (
            """INSERT INTO SCORES(USER_ID,SCORETYPE_ID,GAME_ID,VALUE)
            VALUES(5,2,3,50),
                  (5,2,2,55),
                  (5,1,1,56),
                  (5,1,2,15),
                  (5,1,3,18),
                  (5,1,2,19),
                  (5,2,1,91),
                  (5,2,2,51),
                  (5,1,3,81),
                  (5,2,2,59),
                  (5,2,1,5),
                  (5,1,2,15)
            """
        )
    # query = """
    # DELETE FROM SCORES
    # WHERE GAME_ID = 3 AND SC;
    # """
    database.execute_query(query)
    