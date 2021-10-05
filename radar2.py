from tkinter import *
import time
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler

altura = 2

janela = Tk()
janela.title('radar')
janela.geometry('800x800')

canvas = Canvas(janela, width=800, height=800, bg='white')
canvas.pack()


def map(x: int, in_min: int, in_max: int, out_min: int, out_max: int) -> int:
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min


def converter_coordenadas(xy: tuple) -> tuple:
    x = map(xy[0], -400, 400, 0, 400)
    y = map(xy[1], -400, 400, 0, 400)
    return (x, y)


def desenhar_quadrado(localizacao: tuple, altura: int, cor: str):
    global canvas
    x, y = converter_coordenadas(localizacao)
    print(x, y)
    canvas.create_rectangle(
        x,
        y,
        x+altura,
        y+altura,
        fill=cor
    )


patterns = ['*']
ignore_patterns = None
ignore_directories = False
case_sensitive = True
my_event_handler = PatternMatchingEventHandler(
    patterns, ignore_patterns, ignore_directories, case_sensitive
)


def on_modified(event):
    global altura
    if event.src_path.endswith('.txt'):
        canvas.delete("all")
        with open('./leituras.txt', 'r') as f:
            lines = f.readlines()
            for line in lines:
                data = line.replace('\n', '')
                data = line.split('; ')
                x = data[0].replace('\n', '')
                y = data[1].replace('\n', '')
                x = int(float(x.replace(',', '.')))
                y = int(float(y.replace(',', '.')))
                desenhar_quadrado((x, y), altura, 'black')
            return


my_event_handler.on_modified = on_modified


path = "./"
go_recursively = True
my_observer = Observer()
my_observer.schedule(my_event_handler, path, recursive=go_recursively)

with open('./leituras.txt', 'r') as f:
    lines = f.readlines()
    for line in lines:
        data = line.replace('\n', '')
        data = line.split('; ')
        x = data[0].replace('\n', '')
        y = data[1].replace('\n', '')
        x = int(float(x.replace(',', '.')))
        y = int(float(y.replace(',', '.')))
        desenhar_quadrado((x, y), altura, 'black')

my_observer.start()
janela.mainloop()
try:
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    my_observer.stop()
    my_observer.join()