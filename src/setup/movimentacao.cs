// Métodos de movimentação e outros

void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void rotacionar(int velocidade, int graus) => bc.MoveFrontalAngles(velocidade, graus);
void encoder(int velocidade, float rotacoes) => bc.MoveFrontalRotations(velocidade, rotacoes);
void parar() { bc.MoveFrontal(0, 0); delay(10); }
void travar() { bc.MoveFrontal(0, 0); delay(999999999); }

// Curva para a esquerda em graus
void girar_esquerda(int graus)
{
    float objetivo = converter_graus(eixo_x() - graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Curva para a direita em graus
void girar_direita(int graus)
{
    float objetivo = converter_graus(eixo_x() + graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Gira para a esquerda até um objetivo (bússola)
void objetivo_esquerda(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Alinha o robô no ângulo reto mais próximo
void alinhar_angulo()
{
    led(255, 255, 0);
    print(2, "Alinhando robô");

    int alinhamento = 0;
    float angulo = eixo_x();

    if (angulo_reto())
    {
        return;
    }

    if ((angulo > 315) || (angulo <= 45))
    {
        alinhamento = 0;
    }
    else if ((angulo > 45) && (angulo <= 135))
    {
        alinhamento = 90;
    }
    else if ((angulo > 135) && (angulo <= 225))
    {
        alinhamento = 180;
    }
    else if ((angulo > 225) && (angulo <= 315))
    {
        alinhamento = 270;
    }

    angulo = eixo_x();

    if ((alinhamento == 0) && (angulo > 180))
    {
        objetivo_direita(alinhamento);
    }
    else if ((alinhamento == 0) && (angulo < 180))
    {
        objetivo_esquerda(alinhamento);
    }
    else if (angulo < alinhamento)
    {
        objetivo_direita(alinhamento);
    }
    else if (angulo > alinhamento)
    {
        objetivo_esquerda(alinhamento);
    }

    limpar_linha(2);
}

// Ajusta os sensores na linha preta
void ajustar_linha()
{
    led(255, 255, 0);


    tempo_correcao = millis() + 150;
    while (cor(0) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(-1000, 1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(1) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(-1000, 1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(3) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(1000, -1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(2) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(1000, -1000);
    }

    parar();
}
