// Métodos de leitura e outros

int millis() => (int)(bot.Timer());
bool toque() => (bot.Touch(0));
string cor(byte sensor) => bot.ReturnColor(sensor);
int luz(byte sensor) => (int)bot.Lightness(sensor);
int ultra(byte sensor) => (int)bot.Distance(sensor);
float eixo_x() => bot.Compass();
float eixo_y() => bot.Inclination();
float angulo_atuador() => bot.AngleActuator();
float angulo_giro_atuador() => bot.AngleScoop();
bool tem_vitima() => bot.HasVictim();
void delay(int milissegundos) => bot.Wait(milissegundos);

void som(string nota, int tempo) => bot.PlayNote(0, nota, tempo);

Dictionary<string, string> cores = new Dictionary<string, string>(){
    {"preto", "#000000"},
    {"cinza escuro", "#333332"},
    {"cinza claro", "#656565"},
    {"branco", "#fffffe"},
    {"amarelo", "#ffcc02"},
    {"verde", "#009245"},
    {"vermelho", "#ff3232"},
    {"azul", "#28ade2"}
};
void led(byte R, byte G, byte B) => bot.TurnLedOn(R, G, B);
void led(string cor)
{
    if (cor == "desligado")
    {
        bot.TurnLedOff();
        return;
    }
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    bot.TurnLedOn(cor);
}

void console_led(byte linha, object texto, string cor, bool ligar_led = true)
{
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    string texto_aux = texto.ToString();
    texto_aux = texto_aux.Replace("<:", $"<color={cor}>");
    texto_aux = texto_aux.Replace(":>", "</color>");
    print(linha, "<align=center>" + texto_aux.ToString() + "</align>");
    if (!ligar_led)
    {
        bot.TurnLedOff();
        return;
    }
    bot.TurnLedOn(cor);
}


void print(byte linha, object texto) { if (console) bot.Print(linha - 1, "<align=center>" + texto.ToString() + "</align>"); bot.Print(linha, ""); }

void limpar_console() => bot.ClearConsole();
void limpar_linha(byte linha) => print((byte)(linha - 1), " ");

bool tem_linha(byte sensor) => (bot.returnRed(sensor) < 24);

bool vermelho(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 66, media_verde = 16, media_azul = 16;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERMELHO");
}

/* bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    print(1, $"{vermelho} | {verde} | {azul}");
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96) || cor(sensor) == "VERDE");
} */

bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 13, media_verde = 82, media_azul = 4;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERDE");
}

bool preto(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) < media_meio) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) < 40) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    return false;
}

bool branco(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) > media_meio) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) > 40) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    return false;
}

void calibrar()
{
    alinhar_linha(true);
    media_meio = (byte)Math.Round((luz(1) + luz(2)) / 3f);

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

}

void verifica_calibrar()
{
    if (proximo(eixo_x(), saida1))
    {
        calibrar();
    }

    else if (proximo(eixo_x(), saida2))
    {
        calibrar();
    }
}

void ler_cor()
{
    preto0 = preto(0);
    preto1 = preto(1);
    preto2 = preto(2);
    preto3 = preto(3);

    verde0 = verde(0);
    verde1 = verde(1);
    verde2 = verde(2);
    verde3 = verde(3);

    preto_curva_dir = (preto(0));
    preto_curva_esq = (preto(3));
}

void ler_ultra()
{
    ultra_frente = ultra(0);
    ultra_direita = ultra(1);
    ultra_esquerda = ultra(2);
}

bool angulo_reto()
{
    foreach (int angulo_verificado in angulos_retos)
    {
        if (proximo(eixo_x(), angulo_verificado))
        {
            return true;
        }
    }
    return false;
}
