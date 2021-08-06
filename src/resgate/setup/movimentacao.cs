// metodos de movimentação para a area de resgate
//;
void alinhar_ultra(int distancia, bool empinada = true)
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
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima())
    {
        if (millis() > timeout_vitima)
        {
            fechar_atuador();
            levantar_atuador();
            if (!tem_vitima()) { return; }
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