import paramiko
import time

hosts = ['ec2-54-191-220-240.us-west-2.compute.amazonaws.com', 'ec2-54-191-56-131.us-west-2.compute.amazonaws.com',
         'ec2-54-191-78-13.us-west-2.compute.amazonaws.com']

if __name__ == '__main__':

    ssh = paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    print hosts[0]
    ssh.connect(hosts[0], username='ubuntu', key_filename='/Users/mateusz/Downloads/mbook.pem.txt')
    ssh.exec_command('cd /dht-server/bin/Debug && export PUBLIC_URL="http://%s:8888/" && screen -dmS app mono server.exe' % hosts[0])
    ssh.close()

    for i in range(1, len(hosts)):
        print hosts[i]
        ssh = paramiko.SSHClient()
        ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        ssh.connect(hosts[0], username='ubuntu', key_filename='/Users/mateusz/Downloads/mbook.pem.txt')
        command = 'cd /dht-server/bin/Debug && export PUBLIC_URL="http://%s:8888/" && export PARENT_URL="http://%s:8888/" && screen -dmS app mono server.exe' % (
        hosts[i], hosts[i - 1])
        print command
        ssh.exec_command(command)
        ssh.close()
