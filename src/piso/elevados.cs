bool verifica_gangorra()
{
    if (eixo_y() > 350 || eixo_y() < 5)
    {
        alinhar_angulo();
        parar();
        console_led(1, "<:GANGORRA:>", "vermelho");
        encoder(-300, 2);
        delay(600);
        alinhar_angulo();
        return true;
    }
    return false;
}

bool verifica_rampa()
{
    /* 

        Quando a inclinação for próxima de 350
            Levanta o atuador
            Sobe e para o tempo suficiente pra possível gangorra
            Abaixa o atuador e retorna

    */
    if (millis() < update_rampa)
        return false;

    if (proximo(eixo_y(), 350))
    {
        parar();
        levantar_atuador();
        int tempo_subir = millis() + 2500;
        bool flag_subiu = false;
        int tempo_check_gangorra = millis() + 400;
        while (millis() < tempo_subir)
        {
            if (millis() > tempo_check_gangorra && proximo(eixo_y(), 340))
            {
                flag_subiu = true;
            }
            if (flag_subiu && verifica_gangorra()) { break; }
            ultima_correcao = millis();
            seguir_linha();
            if (verifica_rampa_resgate())
                return true;
        }
        abaixar_atuador();
        update_rampa = millis() + 2000;
        return true;
    }
    return false;

}

bool verifica_rampa_resgate()
{
    if ((proximo(eixo_y(), 340, 10)) && (ultra(1) < 40 && ultra(2) < 40))
    {
        lugar = "rampa resgate";
        return true;
    }
    return false;
}
