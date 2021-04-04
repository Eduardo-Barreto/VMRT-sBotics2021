import os
import datetime

os.system('cls')

directory_rc = ['utils', 'base', 'seguir_linha', 'main']
directory_robo3 = ['utils', 'base', 'seguir_linha', 'main']

rc = open('./RC/out/main.cs', 'w', encoding="utf8")
robo3 = open('./Robo 3/out/main.cs', 'w', encoding="utf8")

data = datetime.datetime.now()

rc.write('/*Last change: ' + str(data.strftime("%d")) + '/' + str(data.strftime("%m")
                                                                  ) + '/' + str(data.strftime("%Y")) + ' | ' + str(data.strftime("%X")) + '\n')
robo3.write('/*Last change: ' + str(data.strftime("%d")) + '/' + str(data.strftime("%m")
                                                                     ) + '/' + str(data.strftime("%Y")) + ' | ' + str(data.strftime("%X")) + '\n')
rc.write('-'*100+'*/\n\n')
robo3.write('-'*100+'*/\n\n')

os.system('cls')

for source_file in directory_rc:
    current_file = open(f'./RC/src/{source_file}.cs', 'r', encoding="utf8")
    for line in current_file:
        rc.write(line)
    current_file.close()
    rc.write('\n\n')

for source_file in directory_robo3:
    current_file = open(f'./Robo 3/src/{source_file}.cs', 'r', encoding="utf8")
    for line in current_file:
        robo3.write(line)
    current_file.close()
    robo3.write('\n\n')

rc.close()
robo3.close()
print('\nCompilado com sucesso.\n')
