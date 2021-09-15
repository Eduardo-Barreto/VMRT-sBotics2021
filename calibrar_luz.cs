float maximo = 0,
    timeout = 0,
    minimo = 100;

void Main(){
    bot.ActuatorSpeed(150);
    bot.ActuatorUp(600);
    bot.Move(1000, -1000);
    bot.Wait(100);

    while (bot.Compass() < 45)
    {
        bot.Move(1000, -1000);
        maximo = (bot.Lightness(0) > maximo) ? bot.Lightness(0) : maximo;
        maximo = (bot.Lightness(1) > maximo) ? bot.Lightness(1) : maximo;
        maximo = (bot.Lightness(2) > maximo) ? bot.Lightness(2) : maximo;
        maximo = (bot.Lightness(3) > maximo) ? bot.Lightness(3) : maximo;

        minimo = (bot.Lightness(0) < minimo) ? bot.Lightness(0) : minimo;
        minimo = (bot.Lightness(1) < minimo) ? bot.Lightness(1) : minimo;
        minimo = (bot.Lightness(2) < minimo) ? bot.Lightness(2) : minimo;
        minimo = (bot.Lightness(3) < minimo) ? bot.Lightness(3) : minimo;
        bot.Print(1, $"min: {minimo} | max: {maximo}");
    }
    timeout = bot.Timer() + 10000;
    while (bot.Timer() < timeout)
    {
        bot.Move(200, 200);
        maximo = (bot.Lightness(0) > maximo) ? bot.Lightness(0) : maximo;
        maximo = (bot.Lightness(1) > maximo) ? bot.Lightness(1) : maximo;
        maximo = (bot.Lightness(2) > maximo) ? bot.Lightness(2) : maximo;
        maximo = (bot.Lightness(3) > maximo) ? bot.Lightness(3) : maximo;

        minimo = (bot.Lightness(0) < minimo) ? bot.Lightness(0) : minimo;
        minimo = (bot.Lightness(1) < minimo) ? bot.Lightness(1) : minimo;
        minimo = (bot.Lightness(2) < minimo) ? bot.Lightness(2) : minimo;
        minimo = (bot.Lightness(3) < minimo) ? bot.Lightness(3) : minimo;
        bot.Print(1, $"min: {minimo} | max: {maximo}");
    }
    bot.Move(1000, -1000);
    bot.Wait(100);
    while((bot.Compass() > 46) || (bot.Compass() < 44))
    {
        bot.Move(1000, -1000);
        maximo = (bot.Lightness(0) > maximo) ? bot.Lightness(0) : maximo;
        maximo = (bot.Lightness(1) > maximo) ? bot.Lightness(1) : maximo;
        maximo = (bot.Lightness(2) > maximo) ? bot.Lightness(2) : maximo;
        maximo = (bot.Lightness(3) > maximo) ? bot.Lightness(3) : maximo;

        minimo = (bot.Lightness(0) < minimo) ? bot.Lightness(0) : minimo;
        minimo = (bot.Lightness(1) < minimo) ? bot.Lightness(1) : minimo;
        minimo = (bot.Lightness(2) < minimo) ? bot.Lightness(2) : minimo;
        minimo = (bot.Lightness(3) < minimo) ? bot.Lightness(3) : minimo;
        bot.Print(1, $"min: {minimo} | max: {maximo}");
    }
    timeout = bot.Timer() + 8000;
    while (bot.Timer() < timeout)
    {
        bot.Move(200, 200);
        maximo = (bot.Lightness(0) > maximo) ? bot.Lightness(0) : maximo;
        maximo = (bot.Lightness(1) > maximo) ? bot.Lightness(1) : maximo;
        maximo = (bot.Lightness(2) > maximo) ? bot.Lightness(2) : maximo;
        maximo = (bot.Lightness(3) > maximo) ? bot.Lightness(3) : maximo;

        minimo = (bot.Lightness(0) < minimo) ? bot.Lightness(0) : minimo;
        minimo = (bot.Lightness(1) < minimo) ? bot.Lightness(1) : minimo;
        minimo = (bot.Lightness(2) < minimo) ? bot.Lightness(2) : minimo;
        minimo = (bot.Lightness(3) < minimo) ? bot.Lightness(3) : minimo;
        bot.Print(1, $"min: {minimo} | max: {maximo}");
    }
    bot.Move(1000, -1000);
    bot.Wait(100);
    while((bot.Compass() > 46) || (bot.Compass() < 44))
    {
        bot.Move(1000, -1000);
        maximo = (bot.Lightness(0) > maximo) ? bot.Lightness(0) : maximo;
        maximo = (bot.Lightness(1) > maximo) ? bot.Lightness(1) : maximo;
        maximo = (bot.Lightness(2) > maximo) ? bot.Lightness(2) : maximo;
        maximo = (bot.Lightness(3) > maximo) ? bot.Lightness(3) : maximo;

        minimo = (bot.Lightness(0) < minimo) ? bot.Lightness(0) : minimo;
        minimo = (bot.Lightness(1) < minimo) ? bot.Lightness(1) : minimo;
        minimo = (bot.Lightness(2) < minimo) ? bot.Lightness(2) : minimo;
        minimo = (bot.Lightness(3) < minimo) ? bot.Lightness(3) : minimo;
        bot.Print(1, $"min: {minimo} | max: {maximo}");
    }
    bot.Print(2, "finalizado");
    bot.Move(0, 0);
}