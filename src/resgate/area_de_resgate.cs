void area_de_resgate()
{
    bot.SetFileConsolePath("C:/Users/samoc/Desktop/VMRT-sBotics2021/leituras.txt");

    mover_tempo(300, 2043); // entra na sala
    varrer_mapear(); // varre em 360° e gera o mapa em xy

    identificar_robo(); // identifica a cordenada do robô 
    definir_parede(); // identifica as paredes
    identificar_saida(); // identifica a saida e a entrada
    verificar_salvamento(); // identifica onde está o triangulo 
    procurar_parede_resgate(); // define em qual parede o robo iniciara o resgate

    //bot.EraseConsoleFile();
    //desenhar();

    if (tem_kit()) // caso o robô tenha entrado com o kit de resgate ele ira entregalo
    {
        mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
        alinhar_angulo_45();
        mover_tempo(300, 255);
        alinhar_angulo_45();
        entregar_vitima();
    }

    mover_xy_costas(xy_resgate[x_baixo], xy_resgate[y_baixo]); // o robô vai até a parede inicial do resgate 
    if (xy_resgate[x_baixo] == 0) // condição para se alinhar para o lado correto conforme a parede escolhida
    {
        girar_objetivo(90);
    }
    else
    {
        girar_objetivo(270);
    }
    alinhar_angulo();
    mover_tempo(-300, 191);
    alinhar_angulo();
    mover_tempo(300, 191);
    alinhar_angulo();
    while (true)
    {
        varredura_linear();
    }
}

void desenhar(byte objeto = 0)
{
    if (objeto == 0)
    {
        registrar($"{xy_entrada[0]}; {xy_entrada[1]}");
        registrar($"{xy_saida[0]}; {xy_saida[1]}");
        registrar($"{xy_robo[0]}; {xy_robo[1]}");

        for (int i = 0; i < 360; i++)
        {
            registrar($"{xy_zerado[i, x_baixo]}; {xy_zerado[i, y_baixo]}");
            registrar($"{xy_zerado[i, x_alto]}; {xy_zerado[i, y_alto]}");
            // registrar($"{distancia_grau[i, x_baixo]}; {i}");
        }
    }
    else if (objeto == parede)
    {
        for (int i = 0; i < 360; i++)
        {
            if ((xy_zerado[i, objeto_alto] == parede))
            {
                registrar($"{xy_zerado[i, x_alto]}; {xy_zerado[i, y_alto]}");
            }
            if ((xy_zerado[i, objeto_baixo] == parede))
            {
                registrar($"{xy_zerado[i, x_baixo]}; {xy_zerado[i, y_baixo]}");
            }
        }
    }
    else if (objeto == saida)
    {
        for (int i = 0; i < 360; i++)
        {
            if ((xy_zerado[i, objeto_alto] == saida))
            {
                registrar($"{xy_zerado[i, x_alto]}; {xy_zerado[i, y_alto]}");
            }
            if ((xy_zerado[i, objeto_baixo] == saida))
            {
                registrar($"{xy_zerado[i, x_baixo]}; {xy_zerado[i, y_baixo]}");
            }
        }
    }
}

void varrer_360()
{
    print(2, "FAZENDO VARREDURA");
    direcao_inicial = eixo_x();
    for (int i = 0; i < 360; i++)
    {
        ler_ultra();
        distancia_grau[(int)converter_graus(i - 90), medida_baixa] = ultra_esquerda + raio_l;
        distancia_grau[i, medida_alto] = ultra_frente + raio_c;
        distancia_grau[i, angulo_leitura] = eixo_x();

        objetivo_direita(converter_graus(direcao_inicial + i));
    }
}

void gerar_xy()
{
    for (int i = 0; i < 360; i++)
    {
        // visão dos sensores laterais
        xy_cru[i, x_baixo] = ((distancia_grau[i, medida_baixa]) * seno(distancia_grau[i, angulo_leitura]));
        xy_cru[i, y_baixo] = ((distancia_grau[i, medida_baixa]) * coseno(distancia_grau[i, angulo_leitura]));
        // visão do sensore superior 
        xy_cru[i, x_alto] = ((distancia_grau[i, medida_alto]) * seno(distancia_grau[i, angulo_leitura]));
        xy_cru[i, y_alto] = ((distancia_grau[i, medida_alto]) * coseno(distancia_grau[i, angulo_leitura]));

        // salva o angulo 
        xy_cru[i, angulo_xy] = distancia_grau[i, angulo_leitura];

    }
}

void mapear_xy()
{
    for (int xy = 2; xy < 4; xy++) // acerta leituras 
    {
        menor_valor = 0;
        for (int i = 0; i < 360; i++)
        {
            if (((xy_cru[i, xy] < menor_valor) && (distancia_grau[i, medida_alto] < 400)
            && proximo(((Math.Abs(xy_cru[i, xy])) + (xy_cru[(int)converter_graus(i - 180), xy])), 300, 3))
            || ((xy_cru[i, xy] < menor_valor) && (distancia_grau[i, medida_alto] < 400)
            && proximo(((Math.Abs(xy_cru[i, xy])) + (xy_cru[(int)converter_graus(i - 180), xy])), 400, 3)))
            {
                menor_valor = xy_cru[i, xy];
                //registrar($"{menor_valor} xy {xy} a {xy_cru[i, angulo_xy]}º ");
            }
        }
        for (int i = 0; i < 360; i++)
        {
            xy_zerado[i, xy - 2] = (Math.Abs(menor_valor) + xy_cru[i, xy - 2]);
            xy_zerado[i, xy] = (Math.Abs(menor_valor) + xy_cru[i, xy]);
        }
    }
    for (int xy = 2; xy < 4; xy++) // acerta leituras 
    {
        for (int i = 0; i < 360; i++)
        {
            if ((xy_zerado[i, xy] < -100) || (xy_zerado[i, xy] > 430))
            {
                xy_zerado[i, x_alto] = 500;
                xy_zerado[i, y_alto] = 500;
            }
            if ((xy_zerado[i, xy - 2] < -100) || (xy_zerado[i, xy - 2] > 430))
            {
                xy_zerado[i, x_baixo] = 500;
                xy_zerado[i, y_baixo] = 500;
            }
        }
    }
}

void varrer_mapear()
{

    qualidade_x = 0;
    qualidade_y = 0;

    varrer_360();
    gerar_xy();

    //checa qualidade do mapeamento
    while (qualidade_x < 30 || qualidade_y < 30)
    {
        mapear_xy();
        bot.EraseConsoleFile();
        //desenhar();
        qualidade_x = 0;
        qualidade_y = 0;
        for (int i = 0; i < 360; i++)
        {
            if (proximo(xy_zerado[i, x_alto], 0, 10)) qualidade_x++;
            if (proximo(xy_zerado[i, y_alto], 0, 10)) qualidade_y++;
        }
        print(3, $"a qualidade da leitura foi de: {qualidade_x} em x e {qualidade_y} em y");
        if (qualidade_x < 30 || qualidade_y < 30)
        {
            mover_tempo(300, 511);
            varrer_360();
            gerar_xy();
        }
    }
}

void identificar_robo()
{
    for (int xy = 2; xy < 4; xy++) // acerta leituras 
    {
        menor_valor = 0;
        for (int i = 0; i < 360; i++)
        {
            if (((xy_cru[i, xy] < menor_valor) && (distancia_grau[i, medida_alto] < 400)
            && proximo(((Math.Abs(xy_cru[i, xy])) + (xy_cru[(int)converter_graus(i - 180), xy])), 300, 3))
            || ((xy_cru[i, xy] < menor_valor) && (distancia_grau[i, medida_alto] < 400)
            && proximo(((Math.Abs(xy_cru[i, xy])) + (xy_cru[(int)converter_graus(i - 180), xy])), 400, 3)))
            {
                menor_valor = xy_cru[i, xy];
                //registrar($"{menor_valor} xy {xy} a {xy_cru[i, angulo_xy]}º ");
            }
        }
        xy_robo[xy - 2] = Math.Abs(menor_valor);
    }
}

void definir_parede()
{
    for (int xy = 2; xy < 4; xy++)
    {
        parede_300 = 0;
        parede_400 = 0;

        for (int i = 0; i < 360; i++)
        {
            if (proximo(xy_zerado[i, xy], 300, 3))
            {
                parede_300++;
            }
            if (proximo(xy_zerado[i, xy], 400, 3))
            {
                parede_400++;
            }
        }

        if (parede_300 > parede_400)
        {
            xy_parede[xy - 2] = 300;
            xy_parede[xy] = 300;
        }
        else
        {
            xy_parede[xy - 2] = 400;
            xy_parede[xy] = 400;
        }
    }

    for (int xy = 2; xy < 4; xy++)
    {
        for (int i = 0; i < 360; i++)
        {
            if (proximo(xy_zerado[i, xy], xy_parede[xy], 6))
            {
                xy_zerado[i, objeto_alto] = parede;
            }
            if (proximo(xy_zerado[i, xy], 0, 6))
            {
                xy_zerado[i, objeto_alto] = parede;
            }
        }
    }

    for (int xy = 0; xy < 2; xy++)
    {
        for (int i = 0; i < 360; i++)
        {
            if (proximo(xy_zerado[i, xy], xy_parede[xy], 6))
            {
                xy_zerado[i, objeto_baixo] = parede;
            }
            if (proximo(xy_zerado[i, xy], 0, 6))
            {
                xy_zerado[i, objeto_baixo] = parede;
            }
        }
    }
}

void identificar_saida()
{
    if (proximo(direcao_inicial, 90, 10))
    {
        xy_entrada[x_baixo] = 0;
        xy_entrada[y_baixo] = xy_robo[y_baixo];
    }
    else if (proximo(direcao_inicial, 0, 10) || proximo(direcao_inicial, 360, 10))
    {
        xy_entrada[x_baixo] = xy_robo[x_baixo];
        xy_entrada[y_baixo] = 0;
    }
    else if (proximo(direcao_inicial, 270, 10))
    {
        xy_entrada[x_baixo] = xy_parede[x_baixo];
        xy_entrada[y_baixo] = xy_robo[y_baixo];
    }
    else if (proximo(direcao_inicial, 180, 10))
    {
        xy_entrada[x_baixo] = xy_robo[x_baixo];
        xy_entrada[y_baixo] = xy_parede[y_baixo];
    }

    for (int i = 0; i < 360; i++)
    {
        if ((xy_zerado[i, x_alto] == 500) || (xy_zerado[i, y_alto] == 500))
        {
            xy_zerado[i, objeto_alto] = saida;
        }
    }
    for (int i = 0; i < 360; i++)
    {
        if ((xy_zerado[i, x_baixo] == 500) || (xy_zerado[i, y_baixo] == 500))
        {
            xy_zerado[i, objeto_baixo] = saida;
        }
    }

    for (int i = 0; i < 360; i++)
    {
        if (xy_zerado[i, objeto_alto] == saida && inicio_saida == 0)
        {
            if (i == 0)
            {
                for (int j = 359; j > 270; j--)
                {
                    if (xy_zerado[j, objeto_alto] == parede)
                    {
                        inicio_saida = j;
                        if ((proximo(xy_entrada[x_baixo], xy_zerado[j, x_alto], 60) && (proximo(xy_entrada[y_baixo], xy_zerado[j, y_alto], 60))))
                        {
                            tag_entrada = 1;
                        }
                        break;
                    }
                }
            }
            else
            {
                inicio_saida = i - 1;
                if ((proximo(xy_entrada[x_baixo], xy_zerado[i, x_alto], 60) && (proximo(xy_entrada[y_baixo], xy_zerado[i, y_alto], 60))))
                {
                    tag_entrada = 1;
                }
            }
        }
        if (inicio_saida != 0 && (xy_zerado[i, objeto_alto] == parede) && (termino_saida == 0))
        {
            termino_saida = i;
            if ((proximo(xy_entrada[x_baixo], xy_zerado[i, x_alto], 60) && (proximo(xy_entrada[y_baixo], xy_zerado[i, y_alto], 60))))
            {
                tag_entrada = 1;
            }
        }
        if (xy_zerado[i, objeto_alto] == saida && (inicio_saida != 0) && (termino_saida != 0) && (inicio_saida2 == 0))
        {
            inicio_saida2 = i - 1;
            if ((proximo(xy_entrada[x_baixo], xy_zerado[i, x_alto], 60) && (proximo(xy_entrada[y_baixo], xy_zerado[i, y_alto], 60))))
            {
                tag_entrada = 2;
            }
        }
        if (xy_zerado[i, objeto_alto] == parede && (inicio_saida != 0)
         && (termino_saida != 0) && (inicio_saida2 != 0) && (termino_saida2 == 0))
        {
            termino_saida2 = i;
            if ((proximo(xy_entrada[x_baixo], xy_zerado[i, x_alto], 60) && (proximo(xy_entrada[y_baixo], xy_zerado[i, y_alto], 60))))
            {
                tag_entrada = 2;
            }
        }
    }

    if (tag_entrada == 2)
    {
        if (proximo(xy_zerado[inicio_saida, x_alto], xy_zerado[termino_saida, x_alto], 15))
        {
            xy_saida[y_baixo] = (Math.Min(xy_zerado[inicio_saida, y_alto], xy_zerado[termino_saida, y_alto])) + 50;
            xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto];
        }
        else
        {
            xy_saida[x_baixo] = (Math.Min(xy_zerado[inicio_saida, x_alto], xy_zerado[termino_saida, x_alto])) + 50;
            xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto];
        }
    }
    else
    {
        if (proximo(xy_zerado[inicio_saida2, x_alto], xy_zerado[termino_saida2, x_alto], 15))
        {
            xy_saida[y_baixo] = (Math.Min(xy_zerado[inicio_saida2, y_alto], xy_zerado[termino_saida2, y_alto])) + 50;
            xy_saida[x_baixo] = xy_zerado[inicio_saida2, x_alto];
        }
        else
        {
            xy_saida[x_baixo] = (Math.Min(xy_zerado[inicio_saida2, x_alto], xy_zerado[termino_saida2, x_alto])) + 50;
            xy_saida[y_baixo] = xy_zerado[inicio_saida2, y_alto];
        }
    }

    if (inicio_saida2 == 0)
    {
        if (proximo(xy_zerado[inicio_saida, x_alto], xy_zerado[termino_saida, x_alto], 15))
        {
            if ((proximo(xy_entrada[y_baixo], xy_zerado[inicio_saida, y_alto], 60)))
            {
                if (xy_zerado[inicio_saida, y_alto] > xy_zerado[termino_saida, y_alto])
                {
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto] - 150;
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto];
                }
                else
                {
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto] + 150;
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto];
                }
            }
            else
            {
                if (xy_zerado[inicio_saida, y_alto] > xy_zerado[termino_saida, y_alto])
                {
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto] - 50;
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto];
                }
                else
                {
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto] + 50;
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto];
                }
            }
        }
        else
        {
            if ((proximo(xy_entrada[x_baixo], xy_zerado[inicio_saida, x_alto], 60)))
            {
                if (xy_zerado[inicio_saida, x_alto] > xy_zerado[termino_saida, x_alto])
                {
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto] - 150;
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto];
                }
                else
                {
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto] + 150;
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto];
                }
            }
            else
            {
                if (xy_zerado[inicio_saida, x_alto] > xy_zerado[termino_saida, x_alto])
                {
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto] - 50;
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto];
                }
                else
                {
                    xy_saida[x_baixo] = xy_zerado[inicio_saida, x_alto] + 50;
                    xy_saida[y_baixo] = xy_zerado[inicio_saida, y_alto];
                }
            }
        }
    }

}

void verificar_salvamento()
{
    for (int i = 0; i < 360; i++)
    {
        if ((xy_zerado[i, x_baixo] < 85) && (xy_zerado[i, y_baixo] < 85) && (xy_zerado[i, x_baixo] > 15) && (xy_zerado[i, y_baixo] > 15))
        {
            xy_triangulo[x_baixo] = 50;
            xy_triangulo[y_baixo] = 50;
        }
        if ((xy_zerado[i, x_baixo] > xy_parede[x_baixo] - 85) && (xy_zerado[i, y_baixo] > xy_parede[y_baixo] - 85)
         && (xy_zerado[i, x_baixo] < xy_parede[x_baixo] - 15) && (xy_zerado[i, y_baixo] < xy_parede[y_baixo] - 15))
        {
            xy_triangulo[x_baixo] = xy_parede[x_baixo] - 50;
            xy_triangulo[y_baixo] = xy_parede[y_baixo] - 50;
        }
        if ((xy_zerado[i, x_baixo] < 85) && (xy_zerado[i, x_baixo] > 15) && (xy_zerado[i, y_baixo] > xy_parede[y_baixo] - 85)
        && (xy_zerado[i, y_baixo] < xy_parede[y_baixo] - 15))
        {
            xy_triangulo[x_baixo] = 50;
            xy_triangulo[y_baixo] = xy_parede[y_baixo] - 50;
        }
        if ((xy_zerado[i, y_baixo] < 85) && (xy_zerado[i, y_baixo] > 15) && (xy_zerado[i, x_baixo] > xy_parede[x_baixo] - 85)
        && (xy_zerado[i, x_baixo] < xy_parede[x_baixo] - 15))
        {
            xy_triangulo[x_baixo] = xy_parede[x_baixo] - 50;
            xy_triangulo[y_baixo] = 50;
        }
    }
}

void procurar_parede_resgate()
{
    if (proximo(xy_entrada[x_baixo], xy_parede[x_baixo], 160))
    {
        xy_resgate[x_baixo] = xy_parede[x_baixo];
    }
    else
    {
        xy_resgate[x_baixo] = 0;
    }

    xy_resgate[y_baixo] = xy_parede[y_baixo] / 2;
    medida_max = (xy_parede[y_baixo] / 2) + 50;
}

void achar_robo()
{
    alinhar_angulo();
    leitura_frente = ultra(0);
    angulo_leitura_frente = eixo_x();

    girar_objetivo(270);
    leitura_lateral = ultra(0);
    xy_robo[x_baixo] = leitura_lateral + raio_c;

    if (leitura_frente >= medida_max)
    {
        girar_objetivo(converter_graus(angulo_leitura_frente + 180));
        leitura_frente = ultra(0);
        angulo_leitura_frente = eixo_x();
    }

    if (proximo(angulo_leitura_frente, 180, 10))
    {
        xy_robo[y_baixo] = leitura_frente + raio_c;
    }
    else
    {
        xy_robo[y_baixo] = xy_parede[y_baixo] - (leitura_frente + raio_c);
    }

    if (leitura_lateral >= xy_parede[x_baixo])
    {
        girar_objetivo(90);
        leitura_lateral = ultra(0);
        xy_robo[x_baixo] = xy_parede[x_baixo] - (leitura_lateral + raio_c);
    }
}

void check_vitima_lateral()
{
    ultra_direita = ultra(1);
    ultra_esquerda = ultra(2);

    if (ultra_direita < medida_max && dir_anterior < medida_max && !proximo(ultra_direita, dir_anterior, 8))
    {
        if (dir_anterior > ultra_direita)
        {
            mover_tempo(300, 111);
        }
        else
        {
            mover_tempo(-300, 255);
        }

        girar_objetivo(converter_graus(eixo_x() + 90));
        viu_vitima = true;
        buscar_vitima();
    }

    if (ultra_esquerda < medida_max && esq_anterior < medida_max && !proximo(ultra_esquerda, esq_anterior, 8))
    {
        if (esq_anterior > ultra_esquerda)
        {
            mover_tempo(300, 111);
        }
        else
        {
            mover_tempo(-300, 255);
        }

        girar_objetivo(converter_graus(eixo_x() - 90));
        viu_vitima = true;
        buscar_vitima();
    }

    dir_anterior = ultra_direita;
    esq_anterior = ultra_esquerda;
}

void buscar_vitima()
{
    timeout = millis() + 4000;
    while ((luz(4) < 52) && (luz(4) > 18) && (millis() < timeout))
    {
        mover(300, 300);
    }
    if (luz(4) >= 52)
    {
        preparar_atuador();
        mover(300, 300);
        delay(799);
        fechar_atuador();
        levantar_atuador();
        mover_tempo(-300, 399);
    }
    else if (luz(4) <= 19)
    {
        flag_vitima_m = true;
        if (contador_vitima >= 2)
        {
            preparar_atuador();
            mover(300, 300);
            delay(799);
            fechar_atuador();
            levantar_atuador();
            mover_tempo(-300, 399);
        }
    }
    else
    {
        mover_tempo(-300, 399);
    }
}

void check_vitima_frente()
{
    if (luz(4) >= 52)
    {
        viu_vitima = true;
        preparar_atuador();
        mover(300, 300);
        delay(799);
        fechar_atuador();
        levantar_atuador();
        mover_tempo(-300, 399);
    }
    else if (luz(4) <= 19)
    {
        flag_vitima_m = true;

        if (contador_vitima >= 2)
        {
            viu_vitima = true;
            preparar_atuador();
            mover(300, 300);
            delay(799);
            fechar_atuador();
            levantar_atuador();
            mover_tempo(-300, 399);
        }
        else
        {
            preparar_atuador();
            mover(300, 300);
            delay(799);
            fechar_atuador();
            levantar_atuador();
            mover_tempo(-300, 399);
            girar_direita(90);
            abaixar_atuador();
            mover_tempo(300, 255);
            mover_tempo(-300, 255);
            levantar_atuador();
            girar_esquerda(90);
            alinhar_angulo();
        }
    }
}

void varredura_linear()
{

    dir_anterior = ultra(1);
    esq_anterior = ultra(2);

    if (xy_parede[x_baixo] == 400) // define o tempo que o robô procurara por vitimas com base na distancia
    {
        tempo_varredura = millis() + 5500;
    }
    else
    {
        tempo_varredura = millis() + 4125;
    }

    while (millis() < tempo_varredura)
    {
        viu_vitima = false;
        check_vitima_lateral();
        if (viu_vitima) { break; }
        check_vitima_frente();
        if (viu_vitima) { break; }
        mover(300, 300);
    }

    achar_robo();

    if (tem_vitima())
    {
        mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
        alinhar_angulo_45();
        mover_tempo(300, 255);
        alinhar_angulo_45();
        entregar_vitima();
        contador_vitima++;
    }

    mover_xy_costas(xy_resgate[x_baixo], xy_resgate[y_baixo]); // o robô vai até a parede inicial do resgate 
    if (xy_resgate[x_baixo] == 0) // condição para se alinhar para o lado correto conforme a parede escolhida
    {
        girar_objetivo(90);
    }
    else
    {
        girar_objetivo(270);
    }
}