// metodos de movimentação para a area de resgate
//;
void alinhar_ultra(int distancia)
{
    if (ultra(0) > distancia)
    {
        while (ultra(0) > distancia + distancia / 6)
        {
            mover(300, 300);
        }
        while (ultra(0) > distancia + distancia / 5)
        {
            mover(200, 200);
        }
        while (ultra(0) > distancia)
        {
            mover(100, 100);
        }
        while (ultra(0) < distancia)
        {
            mover(-75, -75);
        }
    }
    else
    {
        while (ultra(0) < distancia - distancia / 6)
        {
            mover(-300, -300);
        }
        while (ultra(0) < distancia - distancia / 5)
        {
            mover(-200, -200);
        }
        while (ultra(0) < distancia)
        {
            mover(-100, -100);
        }
        while (ultra(0) > distancia)
        {
            mover(75, 75);
        }
    }
    parar();
    delay(5);
}

void entregar_vitima()
{
    abrir_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima())
    {
        if (millis() > timeout_vitima)
        {
            levantar_atuador();
            fechar_atuador();
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    levantar_atuador();
    fechar_atuador();
}

void totozinho(byte vezes = 1)
{ // empurra possiveis bolinhas para frente
    for (byte i = 0; i < vezes; i++)
    {
        encoder(250, 10);
        encoder(-300, 10);
    }
    parar();
}

void preparar_atuador(bool apenas_sem_vitima = false)
{
    if (apenas_sem_vitima)
    {
        if (!tem_vitima())
        {
            totozinho();
            alinhar_angulo();
            abaixar_atuador();
            abrir_atuador();
        }
    }
    else
    {
        totozinho();
        alinhar_angulo();
        abaixar_atuador();
        abrir_atuador();
    }
}