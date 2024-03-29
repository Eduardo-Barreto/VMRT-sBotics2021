// metodos de movimentação para a area de resgate

void alinhar_ultra(float distancia, bool empinada = true)
{
    if (ultra(0) > distancia)
    {
        while (ultra(0) > distancia + distancia / 6)
        {
            mover(300, 300);
            if (empinada) { check_subida_frente(); }
        }
        while (ultra(0) > distancia + distancia / 5)
        {
            mover(200, 200);
            if (empinada) { check_subida_frente(); }
        }
        while (ultra(0) > distancia)
        {
            mover(100, 100);
            if (empinada) { check_subida_frente(); }
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
    girar_baixo_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima() || tem_kit())
    {
        if (millis() > timeout_vitima)
        {
            fechar_atuador();
            levantar_atuador();
            if (!tem_vitima() && !tem_kit()) { return; }
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    fechar_atuador();
    levantar_atuador();
    girar_cima_atuador();
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
            mover_tempo(-300, 639);
            abrir_atuador();
            girar_baixo_atuador();
            abaixar_atuador();
        }
    }
    else
    {
        mover_tempo(-300, 639);
        girar_baixo_atuador();
        abrir_atuador();
        abaixar_atuador();
    }
}

void check_subida_frente(bool alinhar = true)
{
    if (eixo_y() > 330 && eixo_y() < 340)
    {
        if (eixo_y() > 330 && eixo_y() < 340)
        {
            mover_tempo(-200, 255);
            if (alinhar) { alinhar_angulo(); }
        }
    }
}

void mover_travar_tempo(short velocidade = 300, short _timeout = 3000)
{
    /*
    Responsável por mover o robô até ser interrompido por algo externo
        - Move na velocidade indicada
        - Evita problemas com inclinações indesejadas causadas por vítimas
        - Para o robô caso os motores sejam travados ou haja uma alteração grande no ângulo
    */
    // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
    short angulo_inicial = eixo_x();
    // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
    int tempo_check = millis() + 200;
    // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
    bool flag_check = false;
    levantar_atuador();
    timeout = millis() + _timeout;
    while (millis() < timeout)
    {
        // Move o robô
        mover(velocidade, velocidade);
        // Verifica e evita inclinações indesejadas
        check_subida_frente();
        if (!flag_check && millis() > tempo_check)
        {
            // Se a flag era falsa e já passou o tempo inicial
            // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
            flag_check = true;
        }
        if (flag_check && (forca_motor() < 0.3 || !proximo(eixo_x(), angulo_inicial, 3)))
        {
            // Se a flag ja for verdadeira e a força atual for menor que 0.3 ou o angulo atual for muito diferente do angulo inicial
            // Para o loop
            break;
        }
    }
    // Para o robô e alinha o robô no ângulo reto mais próximo
    alinhar_angulo();
}

void mover_travar_ultra(short velocidade = 300, short alvo = 25)
{
    /*
    Responsável por mover o robô até chegar no alvo desejado do ultrassônico
        - Move na velocidade indicada
        - Evita problemas com inclinações indesejadas causadas por vítimas
        - Para o robô caso os motores sejam travados ou haja uma alteração grande no ângulo
    */
    // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
    short angulo_inicial = eixo_x();
    // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
    int tempo_check = millis() + 200;
    // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
    bool flag_check = false;
    levantar_atuador();
    while (ultra(1) > alvo)
    {
        // Move o robô
        mover(velocidade, velocidade);
        // Verifica e evita inclinações indesejadas
        check_subida_frente();
        if (!flag_check && millis() > tempo_check)
        {
            // Se a flag era falsa e já passou o tempo inicial
            // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
            flag_check = true;
        }
        if (flag_check && (forca_motor() < 0.3 || !proximo(eixo_x(), angulo_inicial, 3)))
        {
            // Se a flag ja for verdadeira e a força atual for menor que 0.3 ou o angulo atual for muito diferente do angulo inicial
            // Para o loop
            break;
        }
    }
    // Para o robô e alinha o robô no ângulo reto mais próximo
    alinhar_angulo();
}

void alcancar_saida()
{

    abaixar_atuador();
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
    {
        mover(300, 300);
    }
    limpar_console();
    print(2, "Saindo! Vazando!");
    som("C2", 100);
    timeout = millis() + 1500;
    while (verde(0) || verde(1) || verde(2) || verde(3))
    {
        mover(200, 200);
        if (millis() > timeout)
        {
            levantar_atuador();
            mover_tempo(300, 191);
            break;
        }
    }
    delay(159);
    parar();

    levantar_atuador();

    if (angulo_saida > eixo_x())
    {
        if (((float)Math.Abs(angulo_saida - eixo_x())) > 180)
        {
            while (!preto(1) && !preto(2))
            {
                mover(-1000, 1000);
            }
            parar();
        }
        else
        {
            while (!preto(1) && !preto(2))
            {
                mover(1000, -1000);
            }
            parar();
        }
    }
    else
    {
        if (((float)Math.Abs(angulo_saida - eixo_x())) > 180)
        {
            while (!preto(1) && !preto(2))
            {
                mover(1000, -1000);
            }
            parar();
        }
        else
        {
            while (!preto(1) && !preto(2))
            {
                mover(-1000, 1000);
            }
            parar();
        }
    }
}

void girar_objetivo(float angulo_para_ir)
{
    if (angulo_para_ir > eixo_x())
    {
        if (((float)Math.Abs(angulo_para_ir - eixo_x())) > 180) { objetivo_esquerda(angulo_para_ir); }
        else { objetivo_direita(angulo_para_ir); }
    }
    else
    {
        if (((float)Math.Abs(angulo_para_ir - eixo_x())) > 180) { objetivo_direita(angulo_para_ir); }
        else { objetivo_esquerda(angulo_para_ir); }
    }
}

void mover_xy(float x2, float y2)
{
    direcao_x = x2 - xy_robo[x_baixo];
    direcao_y = y2 - xy_robo[y_baixo];
    angulo_objetivo = (float)((Math.Atan2(direcao_x, direcao_y)) * (180 / Math.PI));
    girar_objetivo(converter_graus(angulo_objetivo));
    distancia_mover_xy = (float)(Math.Sqrt((Math.Pow(direcao_x, 2)) + (Math.Pow(direcao_y, 2))));
    mover_tempo(300, (int)(16 * distancia_mover_xy) - 1);
    xy_robo[x_baixo] = x2;
    xy_robo[y_baixo] = y2;
}

void mover_xy_costas(float x2, float y2)
{
    direcao_x = x2 - xy_robo[x_baixo];
    direcao_y = y2 - xy_robo[y_baixo];
    angulo_objetivo = (float)((Math.Atan2(direcao_x, direcao_y)) * (180 / Math.PI));
    girar_objetivo(converter_graus(angulo_objetivo + 180));
    distancia_mover_xy = (float)(Math.Sqrt((Math.Pow(direcao_x, 2)) + (Math.Pow(direcao_y, 2))));
    mover_tempo(-300, (int)(16 * (int)distancia_mover_xy) - 257, false);
    xy_robo[x_baixo] = x2;
    xy_robo[y_baixo] = y2;
}

void alinhar_angulo_45()
{
    led("amarelo");

    int alinhamento = 0;
    float angulo = eixo_x();

    if ((angulo > 0) && (angulo <= 90))
    {
        alinhamento = 45;
    }
    else if ((angulo > 90) && (angulo <= 180))
    {
        alinhamento = 135;
    }
    else if ((angulo > 180) && (angulo <= 270))
    {
        alinhamento = 225;
    }
    else if ((angulo > 270) && (angulo <= 360))
    {
        alinhamento = 315;
    }

    girar_objetivo(alinhamento);

    led("desligado");
}

void pegar_vitima()
{
    preparar_atuador();
    timeout = millis() + 2000;
    while (millis() < timeout && !tem_vitima())
    {
        mover(300, 300);
        if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
        {
            // Se identificar a fita cinza
            // Para o loop
            break;
        }
        if (verde(0) || verde(1) || verde(2) || verde(3))
        {
            // Se identificar a fita verde
            // Para o loop
            break;
        }
    }
    fechar_atuador();
    girar_cima_atuador();
    levantar_atuador();
    mover_tempo(-300, 399);
}

void alinhar_angulo_90()
{
    led("amarelo");

    int alinhamento = 0;
    float angulo = eixo_x();

    if ((angulo > 0) && (angulo <= 90))
    {
        alinhamento = 0;
    }
    else if ((angulo > 90) && (angulo <= 180))
    {
        alinhamento = 180;
    }
    else if ((angulo > 180) && (angulo <= 270))
    {
        alinhamento = 180;
    }
    else if ((angulo > 270) && (angulo <= 360))
    {
        alinhamento = 0;
    }

    girar_objetivo(alinhamento);

    led("desligado");
}