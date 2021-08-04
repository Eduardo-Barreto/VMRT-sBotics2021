int maximo = 0;
int minimo = 100;
mover(1000, -1000);
delay(100);

while (eixo_x() < 45)
{
    mover(1000, -1000);
    maximo = (luz(0) > maximo) ? luz(0) : maximo;
    maximo = (luz(1) > maximo) ? luz(1) : maximo;
    maximo = (luz(2) > maximo) ? luz(2) : maximo;
    maximo = (luz(3) > maximo) ? luz(3) : maximo;

    minimo = (luz(0) < minimo) ? luz(0) : minimo;
    minimo = (luz(1) < minimo) ? luz(1) : minimo;
    minimo = (luz(2) < minimo) ? luz(2) : minimo;
    minimo = (luz(3) < minimo) ? luz(3) : minimo;
    print(1, $"min: {minimo} | max: {maximo}");
}
timeout = millis() + 10000;
while (millis() < timeout)
{
    mover(200, 200);
    maximo = (luz(0) > maximo) ? luz(0) : maximo;
    maximo = (luz(1) > maximo) ? luz(1) : maximo;
    maximo = (luz(2) > maximo) ? luz(2) : maximo;
    maximo = (luz(3) > maximo) ? luz(3) : maximo;

    minimo = (luz(0) < minimo) ? luz(0) : minimo;
    minimo = (luz(1) < minimo) ? luz(1) : minimo;
    minimo = (luz(2) < minimo) ? luz(2) : minimo;
    minimo = (luz(3) < minimo) ? luz(3) : minimo;
    print(1, $"min: {minimo} | max: {maximo}");
}
for (; ; )
{
    mover(1000, -1000);
    maximo = (luz(0) > maximo) ? luz(0) : maximo;
    maximo = (luz(1) > maximo) ? luz(1) : maximo;
    maximo = (luz(2) > maximo) ? luz(2) : maximo;
    maximo = (luz(3) > maximo) ? luz(3) : maximo;

    minimo = (luz(0) < minimo) ? luz(0) : minimo;
    minimo = (luz(1) < minimo) ? luz(1) : minimo;
    minimo = (luz(2) < minimo) ? luz(2) : minimo;
    minimo = (luz(3) < minimo) ? luz(3) : minimo;
    print(1, $"min: {minimo} | max: {maximo}");
}