void achar_saida()
{
    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo
    limpar_console();
    print(1, "[ ] Alinhando no piso", "left");
    print(2, "[ ] Alinhando no início da arena", "left");
    print(3, "[ ] Verificando saída e triângulo", "left");
    alinhar_angulo();
    // Enquanto não está em um local reto
    while (eixo_y() < 355 && eixo_y() > 5)
    {
        // Anda para frente
        mover(200, 200);
    }
    delay(288);
    print(1, "[X] Alinhando no piso", "left");
    alinhar_angulo();
    // Alinha no início da sala de salvamento
    alinhar_ultra(250, false);
    print(2, "[X] Alinhando no início da arena", "left");
    preparar_atuador();
    alinhar_angulo();
    while (eixo_y() < 355 && eixo_y() > 5)
    {
        // Anda para frente
        mover(200, 200);
    }

    direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate

    mover(250, 250);
    delay(63);
    ler_ultra();
    while (ultra_frente > 180) // enqunto estiver a mais de 180zm da parede frontal busca por saida ou triangulo
    {
        ler_ultra();
        check_subida_frente();
        mover(250, 250);
        if (ultra_direita > 300)  // caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = 3; // determina que a saida está a direita
            print(3, "[X] Verificando saída e triângulo - Saída direita", "left");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = 3; // determina que o triangulo está a direita
            print(3, "[X] Verificando saída e triângulo - Triângulo direita", "left");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }
    if (direcao_triangulo == 0 && direcao_saida == 0)
    {
        print(3, "[X] Verificando saída e triângulo - Nada encontrado", "left");
    }
    ler_ultra();
    print(4, "[ ] Alinhar robô a 150 zm", "left");
    print(5, "[ ] Alinhar robô no triângulo", "left");
    print(6, "[ ] Verificar posição do triângulo", "left");
    while (ultra_frente > 150) // Alinha a 180 zm da parede
    {
        ler_ultra();
        check_subida_frente();
        mover(300, 300);
    }
    print(4, "[X] Alinhar robô a 150 zm", "left");
    fechar_atuador();
    levantar_atuador();
    alinhar_angulo();
    while (ultra_frente > 100) // Alinha a 100 zm da parede
    {
        ler_ultra();
        check_subida_frente();
        if (tem_vitima())
        {
            mover(200, 200);
        }
        else
        {
            mover(300, 300);
        }
    }
    mover_travar_ultra(200, 70);
    mover_travar_tempo(200, 430);
    mover_tempo(150, 430);
    alinhar_angulo();
    print(5, "[X] Alinhar robô no triângulo", "left");
    print(6, $"[X] Verificar posição do triângulo (luz: {luz(4)})", "left");
    if (luz(4) < 2)
    {
        if (tem_vitima())
        {
            mover_tempo(-200, 127);
            girar_direita(45);
            alinhar_ultra(65);
            girar_esquerda(90);
            mover_tempo(200, 127);
            entregar_vitima();
        }
    }
    travar();
}
