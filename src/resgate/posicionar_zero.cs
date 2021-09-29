byte pronto = 0;

void posicionar_zero()
{
    alinhar_angulo();
    mover_tempo(300, 1023); //se desloca para frente para identificar sua posição atual
    ler_ultra(); //checa os ultrasonicos
    if (ultra_frente < 250) // caso a distancia na frente seja menor que 250 o robo apraceu no lado de 4
    {//LADO DE 4
        lado4();
        return;
    }
    else if ((ultra_esquerda + ultra_direita) > 270) // verifica se a soma das distancias é maior que o lado de 3 para ter certeza que esta no lado de 4
    { //LADO DE 4
        lado4();
        return;
    }
    else
    {
        mover_tempo(300, 63);
        ler_ultra();
        if ((ultra_esquerda + ultra_direita) > 270) // efetua a mesma verificação para ter certeza que nenhuma bolinha atrapalhou
        { //LADO DE 4
            lado4();
            return;
        }
        else  // após verificação das laterais do robo se ele não se encaixou em nenhuma é decidido que ele está do lado de 3
        { //LADO DE 3

            return;
        }
    }
}

void lado4()
{
    caso1_4();
    caso2_4();
    caso3_4();
    caso4_4();
}

void caso1_4()
{
    ler_ultra();
    if (ultra_esquerda < 40)
    {
        mover_tempo(300, 63);
        ler_ultra();
        if (ultra_esquerda < 40)
        {
            girar_direita(90);
            alinhar_angulo();
            mover_tempo(-300, 5000);
            alinhar_angulo();
            direcao_inicial = eixo_x();
            direcao_entrada = 0;
            print(2, "Pronto para varredura!");
            pronto = 1;
            return;
        }
    }
}

void caso2_4()
{
    if (pronto == 1) return;
    ler_ultra();
    if (ultra_esquerda < 240)
    {
        mover_tempo(300, 63);
        ler_ultra();
        if (ultra_esquerda < 240)
        {
            if (ultra_esquerda < 140) direcao_entrada = 1;
            else direcao_entrada = 2;
            girar_esquerda(90);
            alinhar_angulo();
            levantar_atuador();
            alinhar_ultra(60); // alinhar proximo ao possivel triangulo 
            alinhar_angulo();
            if (luz(4) < 2)
            {
                direcao_triangulo = 0;
                print(2, "triangulo no ladrilho 0");
            }
            mover_tempo(-300, 511);
            girar_direita(180);
            mover_tempo(-300, 5000);
            alinhar_angulo();
            direcao_inicial = eixo_x();
            print(2, "Pronto para varredura!");
            pronto = 1;
            return;
        }
    }
}

void caso3_4()
{
    if (pronto == 1) return;
    ler_ultra();
    if (ultra_esquerda < 340)
    {
        mover_tempo(300, 63);
        ler_ultra();
        if (ultra_esquerda < 340)
        {
            if (ultra_frente < 250) // verifica se tem parede na frente
            {
                levantar_atuador();
                alinhar_ultra(70); // alinhar proximo ao possivel triangulo 
                if (luz(4) < 2)
                {
                    direcao_triangulo = 0;
                    print(2, "triangulo no ladrilho 0");
                }
                alinhar_angulo();
                alinhar_ultra(80);
                girar_esquerda(45);
                alinhar_ultra(45);
                girar_esquerda(45);
                alinhar_angulo();
                mover_tempo(-300, 2047);
                alinhar_angulo();
                direcao_inicial = eixo_x();
                direcao_entrada = 8;
                print(2, "Pronto para varredura!");
                pronto = 1;
                return;
            }
            else
            {
                direcao_saida = 0;
                direcao_entrada = 8;
                print(2, "saida no ladrilho 0");
                alinhar_angulo();
                ler_cor();
                ler_ultra();
                mover(300, 300);
                while (!verde1 && !verde2 && !verde3 && !verde0 && ultra_esquerda < 400 && ultra_direita < 400)
                {
                    ler_cor();
                    ler_ultra();
                }
                alinhar_angulo();
                mover_tempo(-300, 511);
                girar_esquerda(90);
                alinhar_angulo();
                mover_tempo(-300, 1023);
                direcao_inicial = eixo_x();
                print(2, "Pronto para varredura!");
                pronto = 1;
                return;
            }
        }
    }
}
void caso4_4()
{
    if (pronto == 1) return;
    direcao_saida = 0;
    direcao_entrada = 3;
    print(2, "saida no ladrilho 0");
    alinhar_angulo();
    girar_esquerda(90);
    alinhar_angulo();
    mover_tempo(300, 1023);
    ler_cor();
    ler_ultra();
    mover(300, 300);
    while (!verde1 && !verde2 && !verde3 && !verde0 && ultra_esquerda < 400 && ultra_direita < 400)
    {
        ler_cor();
        ler_ultra();
    }
    alinhar_angulo();
    mover_tempo(-300, 511);
    girar_direita(180);
    alinhar_angulo();
    mover_tempo(-300, 255);
    direcao_inicial = eixo_x();
    print(2, "Pronto para varredura!");
    pronto = 1;
    return;
}