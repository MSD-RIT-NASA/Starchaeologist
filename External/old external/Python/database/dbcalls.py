import logging
import sqlite3
from sqlite3 import Error

class db:
    def __init__(self):
        """
        Connect to database connection
        """
        self.conn = sqlite3.connect("C:/Users/p2201/Downloads/GPBA-main/External/Python/database/Training_System.db",check_same_thread=False)

    def closeConnection(self):
        """
        Close database connection
        """
        self.conn.close()

    def execute_query(self, query):
        """
        Execute query to database
        """
        cursor = self.conn.cursor()
        try:
            cursor.execute(query)
            self.conn.commit()
            logging.info("Query executed successfully")
        except Error as e:
            logging.error(f"The error '{e}' occurred")

    def search_query(self, query):
        """
        Searches database based on query criteria. Returns results
        """
        cursor = self.conn.cursor()
        try:
            cursor.execute(query)
            rows = cursor.fetchall()
            if len(rows) < 1:
                return None
            else:
                return rows
        except Error as e:
            logging.error(f"The error '{e}' occurred")

    def addUser(self, UserName, Password):
        """
        Add user to the database
        """
        query = (
            "INSERT INTO USERS(USERNAME, PASSWORD)"
            "VALUES('"+UserName+"','"+Password+"')"
        )
        self.execute_query(query)
        return self.findUserID(UserName)

    def getGameScore(self, USER_ID):
        """
        Get all previous game scores from the user
        """
        queryOne = (
            "SELECT CAST(strftime('%s', DATE_ADDED) AS INT), VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y "+
            "FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='2' AND GAME_ID='1' ORDER BY DATE_ADDED ASC"
        )
        queryTwo = (
            "SELECT CAST(strftime('%s', DATE_ADDED) AS INT), VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y "+
            "FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='2' AND GAME_ID='2' ORDER BY DATE_ADDED ASC"
        )
        result1 = self.search_query(queryOne)
        result2 = self.search_query(queryTwo)
        return [result1, result2]

    def getBalanceScore(self, USER_ID):
        """
        Get all previous game scores from the user
        """
        queryOne = (
            "SELECT CAST(strftime('%s', DATE_ADDED) AS INT), VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y "+
            "FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='1' AND GAME_ID='1' ORDER BY DATE_ADDED ASC"
        )
        queryTwo = (
            "SELECT CAST(strftime('%s', DATE_ADDED) AS INT), VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y "+
            "FROM SCORES WHERE USER_ID='"+USER_ID+"' AND SCORETYPE_ID='1' AND GAME_ID='2' ORDER BY DATE_ADDED ASC"
        )
        
        result1 = self.search_query(queryOne)
        result2 = self.search_query(queryTwo)
        return [result1, result2]

    def findUserID(self, UserName):
        """
        Find userID number from their username
        """
        query = (
            "SELECT * FROM USERS WHERE USERNAME='"+UserName+"'"
        )
        return self.search_query(query)
        

    def addGameScore(self, USER_ID, GAME_ID, VALUE):
        """
        Add a User's game score to the database
        """
        query = (
            "INSERT INTO SCORES(USER_ID, SCORETYPE_ID, GAME_ID, VALUE)"
            "VALUES("+str(USER_ID)+", 2,"+str(GAME_ID)+","+str(VALUE)+")"
        )
        self.execute_query(query)
        pass

    def addBalanceScore(self, USER_ID, GAME_ID, VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y):
        """
        Add a User's balance score to the database
        """
        query = (
            "INSERT INTO SCORES(USER_ID, SCORETYPE_ID, GAME_ID, VALUE, MEAN_COP, STD_COP, LENGTH_COP, CENTROID_X, CENTROID_Y)"
            "VALUES("+str(USER_ID)+", 1,"+str(GAME_ID)+","+str(VALUE)+","+
            str(MEAN_COP)+","+str(STD_COP)+","+str(LENGTH_COP)+","+str(CENTROID_X)+","+str(CENTROID_Y)+")"
        )
        self.execute_query(query)

    def getTopScores(self):
        """
        Return the top ten game scores for each game
        """
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
        return gameScoreOne, gameScoreTwo

if __name__ == '__main__':
    database = db()
    # query = """
    # ALTER TABLE SCORES ADD COLUMN CENTROID_Y REAL DEFAULT 0.0;
    # """
    # database.execute_query(query)
    database.closeConnection()
    