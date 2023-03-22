# NASA x RIT author: Angela Hudak
import socket

UDP_IP= "192.168.4.4"
UDP_PORT = 4210
MESSAGE = "Hello, World!"



boardSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

boardSock.sendto(bytes(MESSAGE, "utf-8"), (UDP_IP, UDP_PORT))

while True:
    print("####### Server is listening #######")
    boardMsg = boardSock.recvfrom(16)
    print("\n 2. Server received: ", boardMsg[0].decode('utf-8'), "\n")

