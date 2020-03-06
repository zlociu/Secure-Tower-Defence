from hashlib import sha256 
import json

class Player:

    def __init__(self, identity, login, passwdHash, pKey):
        """
        Constructor
        # id = unique number, will be public key
        # login = user's name
        # password = password hashed with SHA256
        """
        self.identity = identity
        self.login = login
        self.password = passwdHash
        self.lvl = 1
        self.points = 0
        self.privateKey = pKey

    @staticmethod
    def hashPassword(passwd):
        return sha256(passwd.encode()).hexdigest()

    def description(self):
        txt = " id: {} \n login: {} \n password: {} \n lvl: {}"
        return txt.format(self.identity, self.login, self.password, self.lvl)


    def playerJSON(self):
        playerString = json.dumps(self.__dict__, sort_keys=True)
        return playerString

        




