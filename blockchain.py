from hashlib import sha256
import json
import time
import player

#from flask import Flask, request
#import requests

class Transaction:

    def __init__(self, player):
        self.player = player.playerJSON()
    
    def transactionJSON(self):
        trString = json.dumps(self.__dict__, sort_keys=True)
        return trString
    
    def compute_hash(self):
        tr_string = self.transactionJSON()
        return sha256(tr_string.encode()).hexdigest()



class Block:
    
    def __init__(self, index, timestamp, previous_hash, transactions, nonce=0):
        """
        Constuctor of Block class.
        # index = ID of block.
        # timestamp = time of adding block.
        # previous_hash = hash of previous block (first don't have).
        # nonce = random value used once.
        """
        self.index = index
        self.timestamp = timestamp
        self.previous_hash = previous_hash
        self.nonce = nonce
        self.transactions = transactions

    def compute_hash(self):
        """
        A function that return the hash of the block.
        """
        block_string = json.dumps(self.__dict__, sort_keys=True)
        return sha256(block_string.encode()).hexdigest()


class Blockchain:
    # difficulty of our PoW (Proof of Work) algorithm
    difficulty = 2

    def __init__(self):
        self.unconfirmed_transactions = []
        self.chain = []

    def create_first_block(self):
        """
        A function to generate first block and appends it to
        the chain. The block has index 0, previous_hash as 0, and
        a valid hash.
        """
        first_block = Block(0, time.time(), 0, [])
        first_block.hash = first_block.compute_hash()
        first_block.numTransactions = 0
        self.chain.append(first_block)

    @property
    def last_block(self):
        return self.chain[-1]

    @property
    def chainLenght(self):
        return len(self.chain)  

    def addBlock(self, block, proof):
        """
        A function that adds the block to the chain after verification.
        Verification includes:
        * Checking if the proof is valid.
        * The previous_hash referred in the block and the hash of latest block
          in the chain match.
        """
        previous_hash = self.last_block.hash

        if previous_hash != block.previous_hash:
            return False

        if not Blockchain.isValidProof(block, proof):
            return False

        block.hash = proof
        self.chain.append(block)
        return True

    @staticmethod
    def proofOfWork(block):
        """
        Function that tries different values of nonce to get a hash
        that satisfies our difficulty criteria.
        """
        block.nonce = 0

        computed_hash = block.compute_hash()
        while not computed_hash.startswith('0' * Blockchain.difficulty):
            block.nonce += 1
            computed_hash = block.compute_hash()

        return computed_hash

    def add_new_transaction(self, transaction):
        self.unconfirmed_transactions.append(transaction)

    @classmethod
    def isValidProof(cls, block, block_hash):
        """
        Check if block_hash is valid hash of block and satisfies
        the difficulty criteria.
        """
        return (block_hash.startswith('0' * Blockchain.difficulty) and
                block_hash == block.compute_hash())

    @classmethod
    def check_chain_validity(cls, chain):
        result = True
        previous_hash = "0"

        for block in chain:
            block_hash = block.hash
            # remove the hash field to recompute the hash again
            # using `compute_hash` method.
            delattr(block, "hash")

            if not cls.isValidProof(block, block_hash) or \
                    previous_hash != block.previous_hash:
                result = False
                break

            block.hash, previous_hash = block_hash, block_hash

        return result

    def mine(self):
        """
        This function serves as an interface to add the pending
        transactions to the blockchain by adding them to the block
        and figuring out Proof Of Work.
        """
        if not self.unconfirmed_transactions:
            return False

        last_block = self.last_block
        new_block = Block(index=last_block.index + 1,
                        transactions=self.unconfirmed_transactions,
                        timestamp=time.time(),
                        previous_hash=last_block.hash)

        new_block.numTransactions = len(self.unconfirmed_transactions)
        proof = self.proofOfWork(new_block)
        self.addBlock(new_block, proof)

        self.unconfirmed_transactions = []

        return True


g1 = player.Player("3", "zlociu", player.Player.hashPassword("0x123456"),"7")
g2 = player.Player("5", "zlociu", player.Player.hashPassword("qwerty"), "11")
g3 = player.Player("7", "aoxter", player.Player.hashPassword("pplubipp"), "19")

t1 = Transaction(g1)
t2 = Transaction(g2)
t3 = Transaction(g3)
t1.hash = t1.compute_hash()
t2.hash = t2.compute_hash()
t3.hash = t3.compute_hash()

blockchain = Blockchain()
blockchain.create_first_block()
blockchain.add_new_transaction(t1.transactionJSON())
blockchain.add_new_transaction(t2.transactionJSON())
blockchain.mine()
blockchain.add_new_transaction(t3.transactionJSON())
blockchain.mine()

print("Chain lenght: ",blockchain.chainLenght)
for block in blockchain.chain:
    #print(block.numTransactions)
    for tr in block.transactions:
        print(tr)


"""
#app = Flask(__name__)

# the node's copy of blockchain
blockchain = Blockchain()
blockchain.create_first_block()

# the address to other participating members of the network
peers = set()


# endpoint to submit a new transaction. This will be used by
# our application to add new data (posts) to the blockchain
@app.route('/new_transaction', methods=['POST'])
def new_transaction():
    tx_data = request.get_json()
    required_fields = ["author", "content"]

    for field in required_fields:
        if not tx_data.get(field):
            return "Invalid transaction data", 404

    tx_data["timestamp"] = time.time()

    blockchain.add_new_transaction(tx_data)

    return "Success", 201


# endpoint to return the node's copy of the chain.
# Our application will be using this endpoint to query
# all the posts to display.
@app.route('/chain', methods=['GET'])
def get_chain():
    chain_data = []
    for block in blockchain.chain:
        chain_data.append(block.__dict__)
    return json.dumps({"length": len(chain_data),
                       "chain": chain_data,
                       "peers": list(peers)})


# endpoint to request the node to mine the unconfirmed
# transactions (if any). We'll be using it to initiate
# a command to mine from our application itself.
@app.route('/mine', methods=['GET'])
def mine_unconfirmed_transactions():
    result = blockchain.mine()
    if not result:
        return "No transactions to mine"
    else:
        # Making sure we have the longest chain before announcing to the network
        chain_length = len(blockchain.chain)
        consensus()
        if chain_length == len(blockchain.chain):
            # announce the recently mined block to the network
            announce_new_block(blockchain.last_block)
        return "Block #{} is mined.".format(blockchain.last_block.index)


# endpoint to add new peers to the network.
@app.route('/register_node', methods=['POST'])
def register_new_peers():
    node_address = request.get_json()["node_address"]
    if not node_address:
        return "Invalid data", 400

    # Add the node to the peer list
    peers.add(node_address)

    # Return the consensus blockchain to the newly registered node
    # so that he can sync
    return get_chain()


@app.route('/register_with', methods=['POST'])
def register_with_existing_node():
    "'""
    Internally calls the `register_node` endpoint to
    register current node with the node specified in the
    request, and sync the blockchain as well as peer data.
    "'""
    node_address = request.get_json()["node_address"]
    if not node_address:
        return "Invalid data", 400

    data = {"node_address": request.host_url}
    headers = {'Content-Type': "application/json"}

    # Make a request to register with remote node and obtain information
    response = requests.post(node_address + "/register_node",
                             data=json.dumps(data), headers=headers)

    if response.status_code == 200:
        global blockchain
        global peers
        # update chain and the peers
        chain_dump = response.json()['chain']
        blockchain = create_chain_from_dump(chain_dump)
        peers.update(response.json()['peers'])
        return "Registration successful", 200
    else:
        # if something goes wrong, pass it on to the API response
        return response.content, response.status_code


def create_chain_from_dump(chain_dump):
    generated_blockchain = Blockchain()
    generated_blockchain.create_first_block()
    for idx, block_data in enumerate(chain_dump):
        if idx == 0:
            continue  # skip first block
        block = Block(block_data["index"],
                      block_data["transactions"],
                      block_data["timestamp"],
                      block_data["previous_hash"],
                      block_data["nonce"])
        proof = block_data['hash']
        added = generated_blockchain.addBlock(block, proof)
        if not added:
            raise Exception("The chain dump is tampered!!")
    return generated_blockchain
*#
"""