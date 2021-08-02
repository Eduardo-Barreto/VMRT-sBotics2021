float direcao_inicial = 0; // variavel para a posição inical do robô

const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
            relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
            sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo


void primeira_busca()
{
    //inicia as localizações zeradas
    direcao_saida = 0;
    direcao_triangulo = 0;
    // Preparações pra alinhamentno na sala
    alinhar_angulo();
    alinhar_angulo();
    preparar_atuador();
    alinhar_ultra(255);
    alinhar_angulo();

    // Busca o triângulo ou a saída na direita
    ler_ultra();
    while (ultra_frente > 180)
    {
        ler_ultra();
        mover(180, 180);
        if (ultra_direita > 300)  // Caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = 3; // Determina que a saida está a direita
            print(1, "SAÍDA DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // Realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = 3; // Determina que o triangulo está a direita
            print(2, "TRIÂNGULO DIREITA");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }
}
