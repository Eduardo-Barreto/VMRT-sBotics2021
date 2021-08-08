void posicao_triangulo2() // posiciona o robo para regate caso o triangulo esteja na posicao 2
{
    alinhar_angulo();
    mover_tempo(-250, 255); // se afasta levemente da parede para virar
    alinhar_ultra(30);
    alinhar_angulo();
    if (tem_vitima()) // caso tenha vitima o robô ira entregá-la antes de se posicionar para o resgate
    {
        objetivo_direita(converter_graus(direcao_inicial + 90)); // robo vira e vai em direção ao triangulo 
        alinhar_ultra(70); // robo se aproxima do triangulo
        alinhar_angulo();
        mover_tempo(150, 511);
        objetivo_esquerda(converter_graus(direcao_inicial + 45)); // gira pra ficar no angulo correto de entregar as vitimas
        mover_tempo(250, 700); // encosta no triangulo 
        entregar_vitima();
        objetivo_esquerda(direcao_inicial); // robo se vira pra ir de costas até a posição de inicio do resgate
        alinhar_ultra(124); // robo vai para o centro da area de resgate
        objetivo_esquerda(converter_graus(direcao_inicial - 90)); // vira pra encostar na parede 
        mover_tempo(-250, 1700); // encosta na parede
        alinhar_angulo(); //alinha o angulo caso tenha esbarrado em uma vitima
        mover_tempo(-300, 255); // força contra a parede caso tenha esbarrado em uma vitima
    }
    else // caso o robô não esteja carregando nenhuma vitima ele ira dar re e ir direto para a posição de resgate
    {
        ler_ultra();
        while (ultra_frente <= 124) // move o robo d ecosta até chegar no meio da area de resgate
        {
            mover(-250, -250);
            ler_ultra();
        }
        alinhar_ultra(124); // alinha o robo com precisão no meio da arena
        objetivo_esquerda(converter_graus(direcao_inicial - 90)); // posiciona o robo de costas para a parede que ele deve encostar
        ler_ultra();
        while (ultra_frente <= 230) // move o robo para tras até ele chegar proximo a parede
        {
            mover(-250, -250);
            ler_ultra();
        }
        mover_tempo(-250, 1000); // encosta o robo na parede
        alinhar_angulo(); // alinha o angulo cado esbarre em alguma vitima 
        mover_tempo(-250, 255); // caso o robo tenha esbarrado em alguma vitima ele ira se forçar contra a parede
    }
}

void posicao_triangulo3() // posiciona o robo para regate caso o triangulo esteja na posicao 3
{
    alinhar_angulo();
    mover_tempo(-250, 255); // se afasta levemente da parede para virar
    alinhar_ultra(30);
    alinhar_angulo();
    if (!tem_vitima()) // caso o robô não esteja carregando nenhuma vitima ele ira direto para o posicionamento de resgate 
    {
        objetivo_direita(converter_graus(direcao_inicial + 90)); // vira a direita                 
        ler_ultra();
        while (ultra_frente >= 124) // enquanto o robô não estiver no meio da area de rasgate move para frente
        {
            mover(250, 250);
            ler_ultra();
        }
        objetivo_esquerda(direcao_inicial); // vira para se alinhar no meio da area 
        while (ultra_frente < 228) // anda para tras até chegar proximo a parede
        {
            mover(-250, -250);
            ler_ultra();
        }
        mover_tempo(-250, 1000); // move até encostar na parede ou dar o tempo de timeout          
        alinhar_angulo(); // alinha o angulo cado esbarre em alguma vitima 
        mover_tempo(-250, 255); // caso o robo tenha esbarrado em alguma vitima ele ira se forçar contra a parede
    }
    else // caso o robô esteja carregando uma vitima ele ira entregala e depois se posicionar para o resgate
    {
        mover_tempo(-250, 255); // se afasta levemente da parede para virar
        objetivo_direita(converter_graus(direcao_inicial + 135)); // faz a curva para o angulo que esta o triangulo 
        preparar_atuador();
        ler_ultra();
        while (ultra_frente >= 105) // move o o robô até se aproximar do triangulo
        {
            mover(250, 250);
            ler_ultra();
        }
        fechar_atuador();
        levantar_atuador();
        mover_tempo(250, 1700); // encosta no triangulo
        entregar_vitima();
        mover_tempo(-250, 511); // se afasta levemente do triangulo para virar 
        objetivo_esquerda(converter_graus(direcao_inicial + 90)); // se virar para ir até o centro da area 
        alinhar_ultra(124); // se alinha no centro da area
        objetivo_esquerda(direcao_inicial); // se vira para encostar na parede
        mover_tempo(-250, 1700); // encosta na parede e está pronto para o resgate
        alinhar_angulo(); // alinha o angulo cado esbarre em alguma vitima 
        mover_tempo(-250, 255); // caso o robo tenha esbarrado em alguma vitima ele ira se forçar contra a parede
    }
}