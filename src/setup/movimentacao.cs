// Métodos de movimentação e outros

void abrir_atuador() => bc.OpenActuator();
void fechar_atuador() => bc.CloseActuator();
void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void encoder(int velocidade, float rotacoes) => bc.MoveFrontalRotations(velocidade, rotacoes);
void parar(int tempo = 10) { bc.MoveFrontal(0, 0); delay(tempo); }
void travar() { bc.MoveFrontal(0, 0); delay(999999999); }

void mover_tempo(int velocidade, int tempo)
{
    int timeout = millis() + tempo;
    while (millis() < timeout)
    {
        mover(velocidade, velocidade);
    }
    parar();
    delay(5);
}

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
void objetivo_esquerda(float objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(float objetivo)
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
    led("amarelo");

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

    led("desligado");
}

// Ajusta os sensores na linha preta
void ajustar_linha(bool por_luz = false)
{
    led("amarelo");

    if (por_luz)
    {
        if (luz(0) < 30 && luz(1) < 30 && luz(2) < 30 && luz(3) < 30)
        {
            mover(200, 200);
            delay(200);
            return;
        }
        tempo_correcao = millis() + 150;
        while (luz(0) < 30 && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(1) < 30 && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(3) < 30 && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(2) < 30 && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
    }
    else
    {
        tempo_correcao = millis() + 150;
        while (cor(0) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(1) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(3) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(2) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
    }

    delay(64);
    parar();
    led("desligado");
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bc.ActuatorSpeed(150);
    bc.ActuatorUp(100);
    if (angulo_atuador() >= 0 && angulo_atuador() < 88)
    {
        bc.ActuatorSpeed(150);
        bc.ActuatorUp(600);
    }
}

void abaixar_atuador()
{
    if (angulo_atuador() > 5)
    {
        bc.ActuatorSpeed(150);
        bc.ActuatorDown(600);
    }
}
