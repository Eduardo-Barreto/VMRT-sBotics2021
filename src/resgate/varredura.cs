void varredura()
{
    bot.SetFileConsolePath("C:/Users/cesar/Desktop/VMRT-sBotics2021/leituras.txt");

    varrer_mapear();
    definir_parede();
    identificar_saida();
    vitimas_centro();
    verificar_salvamento();
    bot.EraseConsoleFile();
    //desenhar();
    bot.EraseConsoleFile();

    if (!tem_kit())
    {
        mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
        entregar_vitima();
        procurar_vitima();
        buscar_vitima();
        travar();
    }

    if (vitima1 != 0)
    {
        pegar_vitima(xy_zerado[vitima1, x_baixo], xy_zerado[vitima1, y_baixo]);
    }
    else if (vitima2 != 0)
    {
        pegar_vitima(xy_zerado[vitima2, x_baixo], xy_zerado[vitima2, y_baixo]);
    }
    else if (vitima3 != 0)
    {
        pegar_vitima(xy_zerado[vitima3, x_baixo], xy_zerado[vitima3, y_baixo]);
    }


    bot.EraseConsoleFile();
    // desenhar();
    print(2, $"{xy_robo[0]}; {xy_robo[1]}");
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
    else if (objeto == 3)
    {

        registrar($"{xy_triangulo[x_baixo]}; {xy_triangulo[y_baixo]}");

        registrar($"{xy_zerado[vitima1, x_baixo]}; {xy_zerado[vitima1, y_baixo]}");
        registrar($"{xy_zerado[vitima2, x_baixo]}; {xy_zerado[vitima2, y_baixo]}");
        registrar($"{xy_zerado[vitima3, x_baixo]}; {xy_zerado[vitima3, y_baixo]}");

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

void varrer_180()
{
    print(2, "FAZENDO VARREDURA");
    direcao_inicial = eixo_x();
    for (int i = 0; i < 180; i++)
    {

        ler_ultra(); // Tira leituras de distancia e salva no array
        distancia_grau[(int)converter_graus(i - 90), medida_baixa] = ultra_esquerda + raio_l;
        distancia_grau[(int)converter_graus(i + 90), medida_baixa] = ultra_direita + raio_l;
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

        //registrar($"{xy_cru[i, x_baixo]}; {xy_cru[i, y_baixo]}");
        //registrar($"{xy_cru[i, x_alto]}; {xy_cru[i, y_alto]}");
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

void vitimas_centro()
{
    tag_vitima1 = 0;
    tag_vitima2 = 0;
    tag_vitima3 = 0;
    for (int i = 0; i < 360; i++)
    {
        if ((xy_zerado[i, objeto_baixo] != parede) && (xy_zerado[i, objeto_baixo] != saida))
        {
            xy_zerado[i, objeto_baixo] = 0;
        }
    }

    for (int i = 0; i < 360; i++)
    {
        if ((xy_zerado[i, x_baixo] > 55) && (xy_zerado[i, x_baixo] < xy_parede[x_baixo] - 55)
        && (xy_zerado[i, y_baixo] > 55) && (xy_zerado[i, y_baixo] < xy_parede[y_baixo] - 55))
        {
            if (tag_vitima1 == 0)
            {
                vitima1 = i;
                tag_vitima1 = 1;
            }
            else if ((tag_vitima2 == 0) && (!proximo(xy_zerado[vitima1, x_baixo], xy_zerado[i, x_baixo], proximidade_vitima))
                                        && (!proximo(xy_zerado[vitima1, y_baixo], xy_zerado[i, y_baixo], proximidade_vitima)))
            {
                vitima2 = i;
                tag_vitima2 = 1;
            }
            else if ((tag_vitima3 == 0)
                  && (!proximo(xy_zerado[vitima1, x_baixo], xy_zerado[i, x_baixo], proximidade_vitima))
                  && (!proximo(xy_zerado[vitima1, y_baixo], xy_zerado[i, y_baixo], proximidade_vitima))
                  && (!proximo(xy_zerado[vitima2, x_baixo], xy_zerado[i, x_baixo], proximidade_vitima))
                  && (!proximo(xy_zerado[vitima2, y_baixo], xy_zerado[i, y_baixo], proximidade_vitima)))
            {
                vitima3 = i;
                tag_vitima3 = 1;
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

        identificar_robo();
        print(1, $"{xy_robo[0]}; {xy_robo[1]}");
    }
}

void pegar_vitima(float x2, float y2)
{
    direcao_x = x2 - xy_robo[x_baixo];
    direcao_y = y2 - xy_robo[y_baixo];
    angulo_objetivo = (float)((Math.Atan2(direcao_x, direcao_y)) * (180 / Math.PI));
    girar_objetivo(converter_graus(angulo_objetivo));
    distancia_mover_xy = (float)(Math.Sqrt((Math.Pow(direcao_x, 2)) + (Math.Pow(direcao_y, 2))));
    mover_tempo(300, (int)((16 * distancia_mover_xy)) - 1);
    xy_robo[x_baixo] = x2;
    xy_robo[y_baixo] = y2;
}

void buscar_vitima()
{
    while ((temperatura() < 37) && (luz(4) > 13))
    {
        mover(300, 300);
    }
    travar();
}

void procurar_vitima()
{
    mover_tempo(-300, 255);
    print(2, "BUSCANDO VITIMAS");
    for (int i = 0; i < 360; i++)
    {
        ler_ultra(); // Tira leituras de distancia e salva no array
        distancia_grau[(int)converter_graus(i + 90), medida_baixa] = ultra_direita + raio_l;
        distancia_grau[i, medida_alto] = ultra_frente + raio_c;
        if ((i > 90) && (i < 267))
        {
            // registrar($"{distancia_grau[i, medida_baixa]}; {distancia_grau[i, medida_alto]}");

            if ((!(proximo(distancia_grau[i, medida_baixa], distancia_grau[i, medida_alto], 16))) // para de usar raio e pegar absoluto da diferença menos 9
            && (distancia_grau[i, medida_baixa] < 400) && (distancia_grau[i, medida_alto] < 400))
            {
                print(1, $"{(int)distancia_grau[i, medida_baixa]}; {(int)distancia_grau[i, medida_alto]}");
                direcao_vitima = i;
                return;
            }
        }
        girar_direita(1);
    }
}