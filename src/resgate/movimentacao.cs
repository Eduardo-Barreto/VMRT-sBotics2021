// metodos de movimentação para a area de resgate
//;
void alinhar_ultra(int distancia, bool empinada = true)
{
    if (ultra(0) > distancia)
    {
        while (ultra(0) > distancia + distancia / 6)
        {
            mover(300, 300);
            if (empinada) { verifica_empinada(); }
        }
        while (ultra(0) > distancia + distancia / 5)
        {
            mover(200, 200);
            if (empinada) { verifica_empinada(); }
        }
        while (ultra(0) > distancia)
        {
            mover(100, 100);
            if (empinada) { verifica_empinada(); }
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
            fechar_atuador();
            levantar_atuador();
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    fechar_atuador();
    levantar_atuador();
}

void totozinho(byte vezes = 1)
{ // empurra possiveis bolinhas para frente
    for (byte i = 0; i < vezes; i++)
    {
        mover_tempo(250, 300);
        mover_tempo(-300, 300);
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
            abrir_atuador();
            abaixar_atuador();
        }
    }
    else
    {
        totozinho();
        alinhar_angulo();
        abrir_atuador();
        abaixar_atuador();
    }
}

void verifica_empinada(bool alinha = true)
{
    if (eixo_y() < 356 && eixo_y() > 330)
    {
        mover_tempo(-200, 399);
        delay(15);
        mover_tempo(200, 399);
        if (alinha) { alinhar_angulo(); }
    }
}

void pegar_vitima()
{
    parar();
    girar_esquerda(90);
    mover_tempo(-300, 511);
    preparar_atuador();
    timeout = millis() + 3000;
    while (!tem_vitima() && millis() < timeout)
    {
        mover(300, 300);
    }
    fechar_atuador();
    levantar_atuador();
    parar();
    if (direcao_triangulo == 2)
    {
        objetivo_esquerda(converter_graus(direcao_inicial + 45));
        mover(300, 300);
    }
}