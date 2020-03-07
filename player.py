from hashlib import sha256 
import json

class Player:

    def __init__(self, identity):
        """
        Constructor
        # id = unique number, will be public key
        # login = user's name
        # password = password hashed with SHA256
        """
        self.identity = identity
        self.lvl = 1
        self.points = 0

    def description(self):
        txt = " id: {} \n lvl: {} \n points: {}"
        return txt.format(self.identity, self.lvl, self.points)


    def playerJSON(self):
        playerString = json.dumps(self.__dict__, sort_keys=True)
        return playerString

        




