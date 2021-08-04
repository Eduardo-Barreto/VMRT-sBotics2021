void resgatar()
{
    const float relacao_sensores_a = -0.9950166112957f,   // constante A da equação para achar o triangulo de resgate
                  relacao_sensores_b = 301.0867774086379f,  // constante B da equação para achar o triangulo de resgate
                  sensibilidade = 5f; // constante de sensibilidade para encontrar triangulo

    if (direcao_triangulo == 2) // condição para caso o triangulo esteja na direita na frente
    {
        ler_ultra();
        while (ultra_frente > 10) // o robô ira realizar a varedura até estar proximo da parede
        {
            mover(300, 300);
            ler_ultra();
            print(1, (ultra_frente * relacao_sensores_a) + relacao_sensores_b);
            print(2, ultra_esquerda);
            while (ultra_frente > 173)
            {
                if (!proximo(ultra_esquerda, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sensibilidade)) // realiza equação y = ax + b para verificar se ha vitimas no lado do triangulo
                {
                    pegar_vitima();
                }
                ler_ultra();
            }
            ler_ultra();
            if (ultra_esquerda < 100)
            {
                pegar_vitima();
            }
            ler_ultra();
        }
    }
}