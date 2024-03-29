
// Declaração das variáveis principais de todo o projeto, separadas por tipos
byte media_meio = 0,
        direcao_triangulo = 255,
        direcao_saida = 255,
        direcao_entrada = 255,
        lugar = 0;

const byte limite_branco = 55;

const short velocidade_max = 250,
        velocidade_padrao = 230;

short velocidade = 250;

float saida1 = 0,
        saida2 = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0,
        direcao_inicial = 0; // variavel para a posição inical do robô no resgate

int tempo_correcao = 0,
        update_time = 16,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0,
        update_curva = 0,
        timeout = 0,
        init_time = 0;

bool preto0 = false,
        preto1 = false,
        preto2 = false,
        preto3 = false,

        verde0 = false,
        verde1 = false,
        verde2 = false,
        verde3 = false,

        preto_curva_dir = false,
        preto_curva_esq = false,

        pegou_kit = false;

short[] angulos_retos = { 0, 90, 180, 270 };

char lado_ajuste = '0';

// Comandos úteis para todo o código

float map(float val, float minimo, float maximo, float minimoSaida, float maximoSaida)
{
    //"mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo, float sensibilidade = 1)
{
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo - sensibilidade && atual < objetivo + sensibilidade);
}

float converter_graus(float graus)
{
    // converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
    return (graus % 360 + 360) % 360;
}

void erro(object texto)
{
    throw new Exception(texto.ToString());
}

string[] rainbow = { "#f90300", "#f89621", "#fce91f", "#42b253", "#2aaae1", "#0047ab", "#9400d3" };

void rainbow_console(string word, string[] colors, int time = 5000)
{

    string word_final = "";
    int colors_index = 0;

    string colorize(char texto, string cor) => $"<color={cor}>{texto}</color>";

    bot.ResetTimer();
    while (bot.Timer() < time)
    {
        colors_index++;

        word_final = "";
        for (byte i = 0; i < word.Length; i++)
        {
            word_final += colorize(word[i], colors[colors_index % colors.Length]);
            bot.TurnLedOn(colors[colors_index % colors.Length]);
            colors_index++;
        }

        bot.Print($"<b><size=60><align=center>{word_final}</align></size></b>\n");
        bot.Wait(200);
    }

}

float coseno(float grau_degree)
{
    return (float)(Math.Cos(Math.PI * grau_degree / 180));
}

float seno(float grau_degree)
{
    return (float)(Math.Sin(Math.PI * grau_degree / 180));
}

void registrar(object texto)
{
    if (registro) bot.WriteText(texto.ToString());
}

// Métodos de leitura e outros

int millis() => (int)(bot.Timer());
bool toque() => (bot.Touch(0));
string cor(byte sensor) => bot.ReturnColor(sensor);
byte luz(byte sensor) => (byte)bot.Lightness(sensor);
short ultra(byte sensor) => (short)bot.Distance(sensor);
short eixo_x() => (short)(bot.Compass());
short eixo_y() => (short)(bot.Inclination());
float angulo_atuador() => bot.AngleActuator();
float angulo_giro_atuador() => bot.AngleScoop();
bool tem_vitima() => bot.HasVictim();
bool tem_kit() => bot.HasRescueKit();
void delay(int milissegundos) => bot.Wait(milissegundos);
float forca_motor() => bot.RobotSpeed();
float temperatura() => (short)bot.Heat();

void som(string nota, short tempo) => bot.PlayNote(0, nota, tempo);

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
    print(linha, texto_aux.ToString());
    if (!ligar_led)
    {
        bot.TurnLedOff();
        return;
    }
    bot.TurnLedOn(cor);
}


void print(byte linha, object texto, string align = "center") { if (console) bot.Print(linha - 1, $"<align={align}>" + texto.ToString()); }

void limpar_console() => bot.ClearConsole();
void limpar_linha(byte linha) => print((byte)(linha - 1), " ");

bool tem_linha(byte sensor) => (bot.returnRed(sensor) < 33);

bool colorido(byte sensor) => (bot.returnRed(sensor) != bot.ReturnBlue(sensor));

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

bool kit_frente()
{
    float val_vermelho = bot.ReturnRed(4);
    float val_verde = bot.ReturnGreen(4);
    float val_azul = bot.ReturnBlue(4);
    byte media_vermelho = 16, media_verde = 34, media_azul = 48;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)));
}

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

bool fita_cinza(int sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 30, media_verde = 32, media_azul = 37;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)));
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
        if ((luz(sensor) < limite_branco) || (cor(sensor) == "PRETO") || tem_linha(sensor))
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
        if ((luz(sensor) > limite_branco) || (cor(sensor) == "BRANCO"))
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

    preto_curva_dir = preto(0);
    preto_curva_esq = preto(3);
}

void ler_ultra()
{
    ultra_frente = ultra(0);
    ultra_direita = ultra(1);
    ultra_esquerda = ultra(2);
}

bool angulo_reto()
{
    foreach (short angulo_verificado in angulos_retos)
    {
        if (proximo(eixo_x(), angulo_verificado))
        {
            return true;
        }
    }
    return false;
}

string luz_marker(int luz)
{
    string hex_str = Convert.ToString(luz, 16);
    string grayscaleHex = hex_str.PadLeft(2, '0');
    string marker = "#" + grayscaleHex + grayscaleHex + grayscaleHex;
    return "<" + "mark=" + marker + ">---" + "<" + "/" + "mark>";
}

void print_luz_marker()
{
    string luz0 = (luz(0).ToString()).PadLeft(3, '0');
    string luz1 = (luz(1).ToString()).PadLeft(3, '0');
    string luz2 = (luz(2).ToString()).PadLeft(3, '0');
    string luz3 = (luz(3).ToString()).PadLeft(3, '0');
    string marker0 = (verde0) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(0)));
    string marker1 = (verde1) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(1)));
    string marker2 = (verde2) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(2)));
    string marker3 = (verde3) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(3)));
    print(2, $"{luz3} | {luz2} | {luz1} | {luz0}");
    print(3, $"{marker3} | {marker2} | {marker1} | {marker0}");
    print(4, "");
}

// Métodos de movimentação e outros

void abrir_atuador() => bot.OpenActuator();
void fechar_atuador() => bot.CloseActuator();
void mover(int esquerda, int direita) => bot.MoveFrontal(direita, esquerda);
void encoder(int velocidade, float rotacoes) => bot.MoveFrontalRotations(velocidade, rotacoes);
void parar(int tempo = 10) { bot.MoveFrontal(0, 0); delay(tempo); }
void travar() { bot.MoveFrontal(0, 0); delay(999999999); }

void mover_tempo(int velocidade, int tempo, bool usar_toque = true, bool usar_forca = true)
{
    timeout = bot.Timer() + tempo;
    int check_forca = millis() + 250;
    while (bot.Timer() < timeout)
    {
        mover(velocidade, velocidade);
        if (usar_toque && velocidade < 0 && toque())
        {
            break;
        }
        if (usar_forca)
        {
            if (millis() > check_forca && forca_motor() < 1)
            {
                break;
            }
        }
    }
    parar();
}

// Curva para a esquerda em graus
void girar_esquerda(int graus)
{
    float objetivo = converter_graus(eixo_x() - graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Curva para a direita em graus
void girar_direita(int graus)
{
    float objetivo = converter_graus(eixo_x() + graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Gira para a esquerda até um objetivo (bússola)
void objetivo_esquerda(float objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(float objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Alinha o robô no ângulo reto mais próximo
void alinhar_angulo()
{
    led("amarelo");

    int alinhamento = 0;
    float angulo = eixo_x();

    if (angulo_reto())
    {
        return;
    }

    if ((angulo > 315) || (angulo <= 45))
    {
        alinhamento = 0;
    }
    else if ((angulo > 45) && (angulo <= 135))
    {
        alinhamento = 90;
    }
    else if ((angulo > 135) && (angulo <= 225))
    {
        alinhamento = 180;
    }
    else if ((angulo > 225) && (angulo <= 315))
    {
        alinhamento = 270;
    }

    angulo = eixo_x();

    if ((alinhamento == 0) && (angulo > 180))
    {
        objetivo_direita(alinhamento);
    }
    else if ((alinhamento == 0) && (angulo < 180))
    {
        objetivo_esquerda(alinhamento);
    }
    else if (angulo < alinhamento)
    {
        objetivo_direita(alinhamento);
    }
    else if (angulo > alinhamento)
    {
        objetivo_esquerda(alinhamento);
    }

    led("desligado");
}

// Ajusta os sensores na linha preta
void alinhar_linha(bool por_luz = false)
{
    led("amarelo");

    if (por_luz)
    {
        if (luz(0) < 30 && luz(1) < 30 && luz(2) < 30 && luz(3) < 30)
        {
            mover(200, 200);
            delay(200);
            return;
        }
        tempo_correcao = millis() + 150;
        while (luz(0) < 30 && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(1) < 30 && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(3) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(2) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
    }
    else
    {
        tempo_correcao = millis() + 150;
        while (cor(0) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(1) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(3) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(2) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
    }

    delay(64);
    parar();
    led("desligado");
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bot.ActuatorSpeed(150);
    bot.ActuatorUp(100);
    if (angulo_atuador() >= 0 && angulo_atuador() < 88)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorUp(600);
    }
}

void abaixar_atuador()
{
    if (angulo_atuador() > 5)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorDown(600);
    }
}

void girar_baixo_atuador()
{
    bot.ActuatorSpeed(150);
    bot.TurnActuatorDown(100);
}

void girar_cima_atuador()
{
    bot.ActuatorSpeed(150);
    bot.TurnActuatorUp(100);
}

bool verifica_saida()
{
    if (lugar == 3)
    {
        if ((vermelho(0)) || (vermelho(1)) || (vermelho(2)) || (vermelho(3)))
        {
            alinhar_angulo();
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 175);
            som("MUDO", 200);

            som("A#2", 112);
            som("A2", 112);
            som("G2", 112);
            som("MUDO", 15);
            som("F2", 300);
            som("MUDO", 150);
            som("F3", 300);
            encoder(300, 15);
            rainbow_console("ARENA FINALIZADA", rainbow);
            travar();
            return true;
        }
        return false;
    }
    // Está saindo da pista (detectou o fim da arena)
    else if (vermelho(1) || vermelho(2))
    {
        console_led(1, "<:Saí da arena...:>", "vermelho");
        som("B3", 64);
        som("MUDO", 16);
        som("B3", 64);
        // Calcula a diferença desde a última correção e vai para trás até encontrar uma linha ou estourar o tempo
        mover(-velocidade, -velocidade);
        delay(150);
        int tras = millis() - ultima_correcao;
        tempo_correcao = millis() + tras;
        while (millis() < tempo_correcao)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                if (!vermelho(1) && !vermelho(2))
                {
                    delay(200);
                    break;
                }
            }
        }
        alinhar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        return true;
    }
    else
    {
        return false;
    }
}

bool verifica_fita_cinza()
{
    if (fita_cinza(1) && fita_cinza(2))
    {
        lugar = 2;
        return true;
    }
    else
    {
        return false;
    }
}


// Segue as linhas
void seguir_linha()
{
    if (verifica_saida()) { return; }
    if (verifica_curva()) { return; }
    console_led(1, $"Seguindo linha: <:{velocidade}:>", "azul", false);
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            alinhar_linha();
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        float objetivo = (lado_ajuste == 'd') ? (converter_graus(eixo_x() + 10)) : (converter_graus(eixo_x() - 10));
        while (!proximo(eixo_x(), objetivo))
        {
            if (lado_ajuste == 'd')
                mover(1000, -1000);
            else
                mover(-1000, 1000);

            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                alinhar_linha(true);
                alinhar_linha();
                ultima_correcao = millis();
                return;
            }
        }
        if (lado_ajuste == 'd')
            girar_esquerda(10);
        else
            girar_direita(10);

        parar();

        // Confirma que está perdido
        console_led(1, "<:Perdi a linha...:>", "vermelho");
        som("F#3", 64);
        som("MUDO", 16);
        som("F#3", 64);
        // Vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() + 1750;
        while (millis() < tras)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                break;
            }
            if (toque())
            {
                parar();
                break;
            }
        }
        delay(150);
        alinhar_linha(true);
        alinhar_linha();
        velocidade = (byte)(velocidade_padrao - 15);
        ultima_correcao = millis();
    }

    // Incremento da velocidade de acordo com o tempo
    if ((millis() > update_time) && (velocidade < velocidade_max))
    {
        update_time = millis() + 32;
        velocidade++;
    }

    // Área do seguidor===============================================================================

    // Se viu preto no sensor da direita
    if (preto1)
    {
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(1) || preto(2))
            {
                break;
            }
            mover(1000, -1000);
        }
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = 'd';
    }

    // Se viu preto no sensor da direita
    else if (preto2)
    {
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(2) || preto(1))
            {
                break;
            }
            mover(-1000, 1000);
        }
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = 'e';
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        verifica_curva();
        mover(velocidade, velocidade);
    }
}

bool falso_verde()
{
    /*
    Falso Verde: Verifica se o robô realmente está no verde e não passou reto de uma outra encruzilhada
        Vem da verificação do verde
        Define um tempo máximo de verificação de 200 milissegundos
        Enquanto está nesse tempo:
            Anda para trás
            Se encontrar a cor preta, vai para frente e retorna verdadeiro (era realmente um falso verde)
            Senão, continua a movimentação, retorna falso (falso falso verde = verde verdadeiro), e realiza a curva
    */
    parar();
    mover_tempo(-180, 63);
    int tempo_check_preto = millis() + 127;
    while (millis() < tempo_check_preto)
    {
        mover(-180, -180);
        if (cor(0) == "PRETO" || cor(3) == "PRETO")
        {
            mover_tempo(300, 288);
            return true;
        }
    }
    mover(200, 200);
    delay(196);
    parar();
    return false;
}

bool beco()
{
    /*
    Beco: Verifica se o robô está no Beco sem saída (verde dos dois lados da encruzilhada)
        Para o robô por tempo suficiente para atualizar a leitura dos sensores
        Se detectar verde dos dois lados:
            Ajusta na linha e para novamente por tempo suficiente para atualizar a leitura dos sensores
            Se novamente identificar verde dos dois lados
                Verifica se é realmente uma encruzilhada (falso_verde())
                Confirmado o beco, acende o led verde e escreve no console
                Indica pelo som que caiu na condição correta
                Vai para frente e faz uma curva de 170 graus para a direita
                Gira até encontrar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Para, lê as cores e verifica se está na condição do beco
    parar();
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        print_luz_marker();
        alinhar_linha();
        delay(64);
        ler_cor();
        print_luz_marker();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            print_luz_marker();
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição
            console_led(1, "<:<b>BECO SEM SAÍDA</b>:>", "verde");
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        return false;
    }
    return false;
}

// Verificação das condições de verde
bool verifica_verde()
{
    /*
    Verifica Verde: Verifica se o robô está em uma encruzilhada com verde
        Atualiza as leituras dos sensores
        Se encontrar verde em algum dos sensores da direita
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à direita
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar verde em algum dos sensores da esquerda
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à esquerda
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        parar();
        alinhar_linha();
        print_luz_marker();
        if (beco()) { return true; }
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Direita", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 431);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }

    // Verifica os sensores da esquerda
    else if (verde2 || verde3)
    {
        parar();
        alinhar_linha();
        print_luz_marker();
        // Verificação do beco sem saída
        if (beco()) { return true; }
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Esquerda", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 431);
            girar_esquerda(30);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}

// Verificações de curvas no preto
bool verifica_curva()
{
    /*
    Verifica Curva: Verifica se o robô está em alguma curva de 90° no preto
        Atualiza as leituras dos sensores
        Verifica se está no verde ou no fim da arena
        Se encontrar preto no sensor da direita
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
            Atualiza a leitura dos sensores
            Se estiver no vermelho (fim da arena), retorna falso (não é curva)
            Se encontrar preto no sensor da esquerda (encruz reta)
                Vai para frente e retorna falso (não é curva)
            Verifica novamente pela saída da arena (vermelho) e verde
            Vai para trás e verifica novamente pelo verde
            Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
            Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
            Com a curva totalmente confirmada, continua girando até achar a linha
            Se passar de 115 graus, asssume que é o ladrilho de Curva C com GAP
                Se alinha atrás e por graus
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
            Se alinha na linha
            Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar preto no sensor da esquerda
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
                Atualiza a leitura dos sensores
                Se estiver no vermelho (fim da arena), retorna falso (não é curva)
                Se encontrar preto no sensor da direita (encruz reta)
                    Vai para frente e retorna falso (não é curva)
                Verifica novamente pela saída da arena (vermelho) e verde
                Vai para trás e verifica novamente pelo verde
                Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
                Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
                Com a curva totalmente confirmada, continua girando até achar a linha
                Se passar de 115 graus, assume que é o ladrilho de Curva C com GAP
                    Se alinha atrás e por graus
                    Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
                Se alinha na linha
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza leituras de cores, verifica se está no verde e depois no vermelho

    if (millis() < update_curva) { return false; }

    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        ler_cor();
        print_luz_marker();
        if (vermelho(0) || colorido(0))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_esq)
        {
            if (verifica_verde()) { return false; }
            // Verifica o verde mais uma vez, vai para trás e verifica novamente
            timeout = millis() + 143;
            while (millis() < timeout)
            {
                mover(-300, -300);
                if (verifica_verde()) { return true; }
            }
            parar();
            mover_tempo(300, 431);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        // Feedbacks visuais e sonoross de que entrou na condição da curva
        console_led(1, "<b>CURVA PRETO</b> - Direita", "preto");
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        mover_tempo(300, 351);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        // Confirmada a curva, gira até encontrar uma linha ou passar do ângulo máximo
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) || vermelho(1))
        {
            mover(1000, -1000);
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 207);
                objetivo = converter_graus(eixo_x() - 115);
                while (!tem_linha(2) || vermelho(2))
                {
                    mover(-1000, 1000);
                    if (proximo(eixo_x(), objetivo))
                    {
                        mover_tempo(-300, 111);
                        while (!tem_linha(1) || vermelho(1))
                        {
                            mover(1000, -1000);
                        }
                        delay(159);
                        alinhar_angulo();
                        parar();
                        mover_tempo(300, 181);
                        velocidade = (byte)(velocidade_padrao - 5);
                        ultima_correcao = millis();
                        calibrar();
                        return true;
                    }
                }
                delay(175);
                parar();
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
        }
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        alinhar_linha();
        alinhar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar(64);
        ler_cor();
        print_luz_marker();
        if (vermelho(3) || colorido(3))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_dir)
        {
            if (verifica_verde()) { return false; }
            // Verifica o verde mais uma vez, vai para trás e verifica novamente
            timeout = millis() + 143;
            while (millis() < timeout)
            {
                mover(-300, -300);
                if (verifica_verde()) { return true; }
            }
            parar();
            mover_tempo(300, 431);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return false; }
        timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        console_led(1, "<b>CURVA PRETO</b> - Esquerda", "preto");
        som("C3", 100);
        mover_tempo(300, 351);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) || vermelho(2))
        {
            mover(-1000, 1000);
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 207);
                objetivo = converter_graus(eixo_x() + 115);
                while (!tem_linha(1) || vermelho(1))
                {
                    mover(1000, -1000);
                    if (proximo(eixo_x(), objetivo))
                    {
                        mover_tempo(-300, 111);
                        while (!tem_linha(2) || vermelho(2))
                        {
                            mover(-1000, 1000);
                        }
                        delay(159);
                        alinhar_angulo();
                        parar();
                        mover_tempo(300, 181);
                        velocidade = (byte)(velocidade_padrao - 5);
                        ultima_correcao = millis();
                        calibrar();
                        return true;
                    }
                }
                delay(175);
                parar();
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
        }
        delay(200);
        alinhar_linha();
        alinhar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }
    else
    {
        return false;
    }
}

bool verifica_obstaculo(bool contar_update = true)
{
    if (contar_update && millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        som("B1", 64);
        som("D2", 64);
        console_led(2, "<:POSSÍVEL OBSTÁCULO:>\n\n", "azul");
        timeout = millis() + 1167;
        while (ultra(0) > 12)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (ultra(0) > 20 && millis() > timeout)
            {
                console_led(1, "<:OBSTÁCULO FALSO:>", "vermelho");
                parar();
                return false;
            }
        }
        parar();
        alinhar_angulo();
        limpar_console();
        som("G2", 64);
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        parar();
        while (ultra(0) > 12)
        {
            mover(75, 75);
        }
        while (ultra(0) < 12)
        {
            mover(-75, -75);
        }
        while (ultra(0) > 12)
        {
            mover(75, 75);
        }
        while (ultra(0) < 12)
        {
            mover(-75, -75);
        }
        parar();

        void alinhar_pos_obstaculo()
        {
            mover_tempo(300, 335);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            alinhar_angulo();
            mover_tempo(-150, 1000);
            alinhar_linha();
            som("D2", 64);
            update_obstaculo = millis() + 50;
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
        }

        void finalizar_desvio_direita()
        {
            som("F#2", 64);
            print(2, "Desvio à direita confirmado!");
            girar_direita(15);
            som("E2", 128);
            mover_tempo(300, 191);
            alinhar_pos_obstaculo();
        }

        void finalizar_desvio_reto()
        {
            som("D2", 64);
            print(2, "Desvio reto confirmado!");
            mover_tempo(300, 127);
            alinhar_pos_obstaculo();
        }

        void finalizar_desvio_esquerda()
        {
            print(2, "Desvio à esquerda confirmado!");
            mover_tempo(300, 127);
            alinhar_pos_obstaculo();
        }


        print(2, "Verificando desvio à direita...");
        girar_direita(50);
        mover_tempo(300, 319);

        int objetivo = 0;
        for (int i = 0; i < 5; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 10));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_direita();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_direita();
                    return true;
                }
            }
            parar();
        }


        print(2, "Verificando desvio reto...");
        som("F#2", 64);

        for (int i = 0; i < 3; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 15));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
        }

        for (int i = 0; i < 3; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 10));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
        }

        print(2, "Verificando desvio à esquerda...");
        som("E2", 64);
        alinhar_angulo();
        mover_tempo(300, 239);
        girar_esquerda(45);

        objetivo = millis() + 271;
        while (millis() < objetivo)
        {
            mover(300, 300);
            if (preto(1) || preto(2))
            {
                break;
            }
        }
        finalizar_desvio_esquerda();
        parar();
        return true;

    }

    return false;
}

bool verifica_gangorra()
{
    /*
    Verifica gangorra: Verifica se o robô está numa gangorra
        Vindo do verifica_rampa, se estiver num ângulo próximo a 0, com 5 de tolerancia
            Alinha no ângulo ortogonal mais próximo
            Escreve no console que está na gangorra e acende o led vermelho
            Vai um pouquinho pra trás e espera 600 milissegundos
            Alinha no ângulo ortogonal mais próximo novamente
            retorna verdadeiro
    */

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
    Verifica rampa: Verifica se o robô está numa rampa
        Quando a inclinação for próxima de 350
            Levanta o atuador
            Define um tempo para chegar ao topo da rampa
            Inicia a subida
                Segue linha
                Verifica se é uma gangorra
            Abaixa o atuador e retorna

    */
    if (millis() < update_rampa)
        return false;

    if (proximo(eixo_y(), 350))
    {
        parar();
        int tempo_subir = millis() + 1781;
        bool flag_subiu = false;
        int tempo_check_gangorra = millis() + 191;
        while (millis() < tempo_subir)
        {
            if (millis() > tempo_check_gangorra && proximo(eixo_y(), 340))
            {
                flag_subiu = true;
            }
            if (flag_subiu && verifica_gangorra()) { break; }
            ultima_correcao = millis();
            seguir_linha();
        }
        parar();
        if (eixo_y() < 10 || eixo_y() > 40)
        {
            timeout = millis() + 303;
            while (eixo_y() < 350 || eixo_y() > 5)
            {
                ultima_correcao = millis();
                seguir_linha();
                if (verifica_obstaculo(false))
                    break;
                if (millis() > timeout)
                    break;

            }
        }
        parar();
        update_rampa = millis() + 2000;
        return true;
    }
    return false;

}

// Variaveis utilizadas no resgate

float[,] distancia_grau = new float[360, 3];
//poss 0 = ultra de baixo 
//poss 1 = ultra de cima
//poss 2 = grau

float[,] xy_cru = new float[360, 5];
//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = angulo

float[,] xy_zerado = new float[360, 6];
//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = objeto_baixo
//poss 5 = objeto_alto

float[] xy_robo = new float[2];
float[] xy_entrada = new float[2];
float[] xy_saida = new float[2];
float[] xy_parede = new float[4];
float[] xy_triangulo = new float[2];
float[] xy_resgate = new float[2];
//poss 0 = x
//poss 1 = y
//poss 2 = x
//poss 3 = y

// referencias para xy_
const byte x_baixo = 0,
           y_baixo = 1,
           x_alto = 2,
           y_alto = 3,
           angulo_xy = 4,

// referencias para xy_zerado e seus objetos    
           objeto_baixo = 4,
           objeto_alto = 5,
           parede = 1,
           triangulo = 2,
           saida = 4,

// referencias para distancia_grau
           medida_baixa = 0,
           medida_alto = 1,
           angulo_leitura = 2,

//medidas do robô
           raio_l = 18,
           raio_c = 28;

int qualidade_x = 0,
    qualidade_y = 0,
    parede_400 = 0,
    parede_300 = 0,
    inicio_saida = 0,
    termino_saida = 0,
    inicio_saida2 = 0,
    termino_saida2 = 0,
    tag_entrada = 0,
    tempo_varredura = 0,
    contador_vitima = 0,
    ignorar_morta = 0,
    ciclos = 0,
    angulo_triangulo,
    angulo_saida,
    contador_timeout=0;

// variaveis para mover_xy e mover_xy_costas
float direcao_x,
      direcao_y,
      angulo_objetivo,
      distancia_mover_xy,
// variavel para as varreduras
      menor_valor = 0,
// variaveis achar robô
      leitura_frente,
      leitura_lateral,
      angulo_leitura_frente,
// variaveis para check_vitima
      dir_anterior,
      esq_anterior,
      medida_max;

bool flag_vitima_m = false,
     viu_vitima = false,
     flag_timeout = false;

// metodos de movimentação para a area de resgate

void alinhar_ultra(float distancia, bool empinada = true)
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
    girar_baixo_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima() || tem_kit())
    {
        if (millis() > timeout_vitima)
        {
            fechar_atuador();
            levantar_atuador();
            if (!tem_vitima() && !tem_kit()) { return; }
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    fechar_atuador();
    levantar_atuador();
    girar_cima_atuador();
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
            mover_tempo(-300, 639);
            abrir_atuador();
            girar_baixo_atuador();
            abaixar_atuador();
        }
    }
    else
    {
        mover_tempo(-300, 639);
        girar_baixo_atuador();
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

void alcancar_saida()
{

    abaixar_atuador();
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
    {
        mover(300, 300);
    }
    limpar_console();
    print(2, "Saindo! Vazando!");
    som("C2", 100);
    timeout = millis() + 1500;
    while (verde(0) || verde(1) || verde(2) || verde(3))
    {
        mover(200, 200);
        if (millis() > timeout)
        {
            levantar_atuador();
            mover_tempo(300, 191);
            break;
        }
    }
    delay(159);
    parar();

    levantar_atuador();

    if (angulo_saida > eixo_x())
    {
        if (((float)Math.Abs(angulo_saida - eixo_x())) > 180)
        {
            while (!preto(1) && !preto(2))
            {
                mover(-1000, 1000);
            }
            parar();
        }
        else
        {
            while (!preto(1) && !preto(2))
            {
                mover(1000, -1000);
            }
            parar();
        }
    }
    else
    {
        if (((float)Math.Abs(angulo_saida - eixo_x())) > 180)
        {
            while (!preto(1) && !preto(2))
            {
                mover(1000, -1000);
            }
            parar();
        }
        else
        {
            while (!preto(1) && !preto(2))
            {
                mover(-1000, 1000);
            }
            parar();
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

void mover_xy_costas(float x2, float y2)
{
    direcao_x = x2 - xy_robo[x_baixo];
    direcao_y = y2 - xy_robo[y_baixo];
    angulo_objetivo = (float)((Math.Atan2(direcao_x, direcao_y)) * (180 / Math.PI));
    girar_objetivo(converter_graus(angulo_objetivo + 180));
    distancia_mover_xy = (float)(Math.Sqrt((Math.Pow(direcao_x, 2)) + (Math.Pow(direcao_y, 2))));
    mover_tempo(-300, (int)(16 * (int)distancia_mover_xy) - 257, false);
    xy_robo[x_baixo] = x2;
    xy_robo[y_baixo] = y2;
}

void alinhar_angulo_45()
{
    led("amarelo");

    int alinhamento = 0;
    float angulo = eixo_x();

    if ((angulo > 0) && (angulo <= 90))
    {
        alinhamento = 45;
    }
    else if ((angulo > 90) && (angulo <= 180))
    {
        alinhamento = 135;
    }
    else if ((angulo > 180) && (angulo <= 270))
    {
        alinhamento = 225;
    }
    else if ((angulo > 270) && (angulo <= 360))
    {
        alinhamento = 315;
    }

    girar_objetivo(alinhamento);

    led("desligado");
}

void pegar_vitima()
{
    preparar_atuador();
    timeout = millis() + 2000;
    while (millis() < timeout && !tem_vitima())
    {
        mover(300, 300);
        if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
        {
            // Se identificar a fita cinza
            // Para o loop
            break;
        }
        if (verde(0) || verde(1) || verde(2) || verde(3))
        {
            // Se identificar a fita verde
            // Para o loop
            break;
        }
    }
    fechar_atuador();
    girar_cima_atuador();
    levantar_atuador();
    mover_tempo(-300, 399);
}

void alinhar_angulo_90()
{
    led("amarelo");

    int alinhamento = 0;
    float angulo = eixo_x();

    if ((angulo > 0) && (angulo <= 90))
    {
        alinhamento = 0;
    }
    else if ((angulo > 90) && (angulo <= 180))
    {
        alinhamento = 180;
    }
    else if ((angulo > 180) && (angulo <= 270))
    {
        alinhamento = 180;
    }
    else if ((angulo > 270) && (angulo <= 360))
    {
        alinhamento = 0;
    }

    girar_objetivo(alinhamento);

    led("desligado");
}

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

    print(1, $"triangulo esta em: {(int)xy_triangulo[x_baixo]}x; {(int)xy_triangulo[y_baixo]}y");

    if (tem_kit()) // caso o robô tenha entrado com o kit de resgate ele ira entregalo
    {
        mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
        alinhar_angulo_45();
        mover_tempo(300, 255);
        alinhar_angulo_45();
        entregar_vitima();
    }

    mover_xy_costas(xy_resgate[x_baixo], xy_resgate[y_baixo]); // o robô vai até a parede inicial do resgate 
    if (xy_resgate[x_baixo] == raio_c) // condição para se alinhar para o lado correto conforme a parede escolhida
    {
        girar_objetivo(90);
    }
    else
    {
        girar_objetivo(270);
    }
    alinhar_angulo();
    mover_tempo(-300, 207);
    alinhar_angulo();
    mover_tempo(300, 207);
    alinhar_angulo();

    ciclos = 0;
    contador_timeout = 0;

    while (ciclos < 5 && contador_vitima < 3 && contador_timeout < 2)
    {
        print(2, $"ciclos de varredura: {ciclos}");
        print(3, $"timeouts: {contador_timeout}");
        varredura_linear();
        ciclos++;
    }
    limpar_console();

    if (contador_vitima < 3)
    {
        passar_vassoura();

        if (tem_vitima())
        {
            achar_robo();
            mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
            girar_objetivo(angulo_triangulo);
            mover_tempo(300, 255);
            girar_objetivo(angulo_triangulo);
            entregar_vitima();
            mover_tempo(-300, 399);
        }
    }

    achar_robo();
    sair(xy_saida[x_baixo], xy_saida[y_baixo]);
    mover_tempo(-300, 63);
    alinhar_linha();
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
    while (qualidade_x < 27 || qualidade_y < 30)
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

    if (xy_saida[x_baixo] == 0)
    {
        angulo_saida = 270;
    }
    else if (xy_saida[x_baixo] == xy_parede[x_baixo])
    {
        angulo_saida = 90;
    }
    else if (xy_saida[y_baixo] == 0)
    {
        angulo_saida = 180;
    }
    else
    {
        angulo_saida = 0;
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
            angulo_triangulo = 225;
        }
        if ((xy_zerado[i, x_baixo] > xy_parede[x_baixo] - 85) && (xy_zerado[i, y_baixo] > xy_parede[y_baixo] - 85)
         && (xy_zerado[i, x_baixo] < xy_parede[x_baixo] - 15) && (xy_zerado[i, y_baixo] < xy_parede[y_baixo] - 15))
        {
            xy_triangulo[x_baixo] = xy_parede[x_baixo] - 50;
            xy_triangulo[y_baixo] = xy_parede[y_baixo] - 50;
            angulo_triangulo = 45;
        }
        if ((xy_zerado[i, x_baixo] < 85) && (xy_zerado[i, x_baixo] > 15) && (xy_zerado[i, y_baixo] > xy_parede[y_baixo] - 85)
        && (xy_zerado[i, y_baixo] < xy_parede[y_baixo] - 15))
        {
            xy_triangulo[x_baixo] = 50;
            xy_triangulo[y_baixo] = xy_parede[y_baixo] - 50;
            angulo_triangulo = 315;
        }
        if ((xy_zerado[i, y_baixo] < 85) && (xy_zerado[i, y_baixo] > 15) && (xy_zerado[i, x_baixo] > xy_parede[x_baixo] - 85)
        && (xy_zerado[i, x_baixo] < xy_parede[x_baixo] - 15))
        {
            xy_triangulo[x_baixo] = xy_parede[x_baixo] - 50;
            xy_triangulo[y_baixo] = 50;
            angulo_triangulo = 135;
        }
    }
}

void procurar_parede_resgate()
{
    if (proximo(xy_entrada[x_baixo], xy_parede[x_baixo], 160))
    {
        xy_resgate[x_baixo] = xy_parede[x_baixo] - raio_c;
    }
    else
    {
        xy_resgate[x_baixo] = raio_c;
    }

    xy_resgate[y_baixo] = xy_parede[y_baixo] / 2;
    medida_max = (xy_parede[y_baixo] / 2) + 50;
}

void achar_robo()
{
    alinhar_angulo_90();
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
        if (!flag_vitima_m)
        {
            girar_objetivo(converter_graus(eixo_x() + 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else if (contador_vitima >= 2)
        {
            girar_objetivo(converter_graus(eixo_x() + 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else if (ignorar_morta >= 2)
        {
            girar_objetivo(converter_graus(eixo_x() + 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else
        {
            mover_tempo(300, 399);
            ignorar_morta++;
        }
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
        if (!flag_vitima_m)
        {
            girar_objetivo(converter_graus(eixo_x() - 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else if (contador_vitima >= 2)
        {
            girar_objetivo(converter_graus(eixo_x() - 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else if (ignorar_morta >= 2)
        {
            girar_objetivo(converter_graus(eixo_x() - 90));
            viu_vitima = true;
            buscar_vitima();
        }
        else //colocar contador maior q 2 vitimas
        {
            mover_tempo(300, 399);
            ignorar_morta++;
        }
    }

    if ((!proximo((ultra_esquerda + ultra_direita), xy_parede[y_baixo], 50) && esq_anterior > medida_max && ultra_esquerda < medida_max)
    || (!proximo((ultra_esquerda + ultra_direita), xy_parede[y_baixo], 50) && dir_anterior > medida_max && ultra_direita < medida_max))
    {
        mover_tempo(300, 111);
        if (ultra_direita < ultra_esquerda)
        {
            if (!flag_vitima_m)
            {
                girar_objetivo(converter_graus(eixo_x() + 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else if (contador_vitima >= 2 || ciclos > 2)
            {
                girar_objetivo(converter_graus(eixo_x() + 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else if (ignorar_morta >= 2)
            {
                girar_objetivo(converter_graus(eixo_x() + 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else
            {
                mover_tempo(300, 399);
                ignorar_morta++;
            }
        }
        else
        {
            if (!flag_vitima_m)
            {
                girar_objetivo(converter_graus(eixo_x() - 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else if (contador_vitima >= 2 || ciclos > 2)
            {
                girar_objetivo(converter_graus(eixo_x() - 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else if (ignorar_morta >= 2)
            {
                girar_objetivo(converter_graus(eixo_x() - 90));
                viu_vitima = true;
                buscar_vitima();
            }
            else //colocar contador maior q 2 vitimas
            {
                mover_tempo(300, 399);
                ignorar_morta++;
            }
        }
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
        if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
        {
            // Se identificar a fita cinza
            // Para o loop
            break;
        }
        if (verde(0) || verde(1) || verde(2) || verde(3))
        {
            // Se identificar a fita verde
            // Para o loop
            break;
        }
    }
    if (luz(4) >= 52)
    {
        pegar_vitima();
    }
    else if (luz(4) <= 19)
    {
        flag_vitima_m = true;
        if (contador_vitima >= 2 || ciclos > 2)
        {
            pegar_vitima();
        }
    }
    else
    {
        mover_tempo(-300, 399);
    }
}

void check_vitima_frente()
{
    if (luz(4) >= 52 && temperatura() > 29)
    {
        viu_vitima = true;
        pegar_vitima();
    }
    else if (luz(4) <= 19)
    {
        flag_vitima_m = true;
        viu_vitima = true;

        if (contador_vitima >= 2)
        {
            pegar_vitima();
        }
        else
        {
            pegar_vitima();
            girar_direita(90);
            abaixar_atuador();
            mover_tempo(300, 255);
            mover_tempo(-300, 255);
            levantar_atuador();
        }
    }
}

void varredura_linear()
{
    alinhar_angulo();
    dir_anterior = ultra(1);
    esq_anterior = ultra(2);

    if (xy_parede[x_baixo] == 400) // define o tempo que o robô procurara por vitimas com base na distancia
    {
        tempo_varredura = millis() + 5700;
    }
    else
    {
        tempo_varredura = millis() + 4400;
    }

    ignorar_morta = 0;
    flag_timeout = false;

    while (true)
    {
        viu_vitima = false;
        check_vitima_lateral();
        if (viu_vitima) { break; }
        check_vitima_frente();
        if (viu_vitima) { break; }
        mover(300, 300);

        if (millis() > tempo_varredura)
        {
            flag_timeout = true;
            contador_timeout++;
            break;
        }
        if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
        {
            mover_tempo(-300, 111);
            // Se identificar a fita cinza
            // Para o loop
            break;
        }
        if (verde(0) || verde(1) || verde(2) || verde(3))
        {
            mover_tempo(-300, 111);
            // Se identificar a fita verde
            // Para o loop
            break;
        }
    }



    if (tem_vitima())
    {
        alinhar_angulo_90();
        achar_robo();
        mover_xy(xy_triangulo[x_baixo], xy_triangulo[y_baixo]);
        girar_objetivo(angulo_triangulo);
        mover_tempo(300, 255);
        girar_objetivo(angulo_triangulo);
        entregar_vitima();
        contador_vitima++;
    }

    if (!flag_timeout)
    {
        alinhar_angulo_90();
        achar_robo();
        mover_xy_costas(xy_resgate[x_baixo], xy_resgate[y_baixo]); // o robô vai até a parede inicial do resgate 
    }
    else
    {
        short angulo_inicial = eixo_x();
        // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
        int tempo_check = millis() + 200;
        // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
        bool flag_check = false;

        timeout = millis() + 5200;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (!flag_check && millis() > tempo_check)
            {
                // Se a flag era falsa e já passou o tempo inicial
                // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
                flag_check = true;
            }

            if (flag_check && forca_motor() < 0.3)
            {
                // Se a flag ja for verdadeira e a força atual for menor que 0.3
                // Para o loop
                break;
            }
            if (flag_check && !((eixo_x() < converter_graus(angulo_inicial + 10)) || (eixo_x() > converter_graus(angulo_inicial - 10))))
            {
                // Se a flag ja for verdadeira e o angulo atual for muito diferente do angulo inicial
                break;
            }
            if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
            {
                // Se identificar a fita cinza
                while ((!fita_cinza(0) && !fita_cinza(1) && !fita_cinza(2) && !fita_cinza(3)))
                {
                    mover(-250, -250);
                }
                while ((fita_cinza(0) || fita_cinza(1) || fita_cinza(2) || fita_cinza(3)))
                {
                    mover(-250, -250);
                }
                delay(239);
                // Para o loop
                break;
            }
            if (verde(0) || verde(1) || verde(2) || verde(3))
            {
                // Se identificar a fita cinza
                while ((!verde(0) && !verde(1) && !verde(2) && !verde(3)))
                {
                    mover(-250, -250);
                }
                while ((verde(0) || verde(1) || verde(2) || verde(3)))
                {
                    mover(-250, -250);
                }
                delay(239);
                // Para o loop
                break;
            }
        }
        parar();
    }


    if (xy_resgate[x_baixo] == raio_c) // condição para se alinhar para o lado correto conforme a parede escolhida
    {
        girar_objetivo(90);
    }
    else
    {
        girar_objetivo(270);
    }
    alinhar_angulo();
    mover_tempo(-300, 207);
    alinhar_angulo();
    mover_tempo(300, 207);
    alinhar_angulo();
}

void sair(float x2, float y2)
{
    direcao_x = x2 - xy_robo[x_baixo];
    direcao_y = y2 - xy_robo[y_baixo];
    angulo_objetivo = (float)((Math.Atan2(direcao_x, direcao_y)) * (180 / Math.PI));
    girar_objetivo(converter_graus(angulo_objetivo));
    alcancar_saida();
    print(2, "Vazei!");
}

void passar_vassoura()
{
    void chegar_final()
    {
        /*
            Chega ao final perto da parede e retorna o porquê da parada
        */

        // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
        short angulo_inicial = eixo_x();
        // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
        int tempo_check = millis() + 200;
        // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
        bool flag_check = false;
        // Flag de verificação configurada como verdadeiro, se ele parar por algo que não foi parede, ela é trocada
        bool parede = true;
        string motivo = "ultrassonico";

        // Enquanto o ultrassônico não identifica parede
        while (ultra(0) > 25)
        {
            // Move o robô
            mover(250, 250);
            if (!flag_check && millis() > tempo_check)
            {
                // Se a flag era falsa e já passou o tempo inicial
                // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
                flag_check = true;
            }

            if (flag_check && forca_motor() < 0.3)
            {
                // Se a flag ja for verdadeira e a força atual for menor que 0.3
                // Para o loop
                motivo = "forca";
                break;
            }

            if (flag_check && !((eixo_x() < converter_graus(angulo_inicial + 10)) || (eixo_x() > converter_graus(angulo_inicial - 10))))
            {
                // Se a flag ja for verdadeira e o angulo atual for muito diferente do angulo inicial
                motivo = "direcao";
                alinhar_angulo();
                break;
            }

            if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
            {
                // Se identificar a fita cinza
                // Declara que não foi a parede que parou
                parede = false;
                motivo = "cinza";
                // Para o loop
                break;
            }
            if (verde(0) || verde(1) || verde(2) || verde(3))
            {
                // Se identificar a fita cinza
                // Declara que não foi a parede que parou
                parede = false;
                motivo = "verde";
                // Para o loop
                break;
            }
        }
        // Volta o atuador para sua posição inicial e para o robô
        fechar_atuador();
        girar_cima_atuador();
        levantar_atuador();
        parar();
        print(2, $"Robô parado por: {motivo}");
        // Se alinha novamente caso não tenha parado pela parede
        if (motivo == "cinza")
        {
            while ((!fita_cinza(0) && !fita_cinza(1) && !fita_cinza(2) && !fita_cinza(3)))
            {
                mover(-250, -250);
            }
            while ((fita_cinza(0) || fita_cinza(1) || fita_cinza(2) || fita_cinza(3)))
            {
                mover(-250, -250);
            }
        }
        if (motivo == "verde")
        {
            while ((!verde(0) && !verde(1) && !verde(2) && !verde(3)))
            {
                mover(-250, -250);
            }
            while ((verde(0) || verde(1) || verde(2) || verde(3)))
            {
                mover(-250, -250);
            }
        }
        delay(239);
        parar();

    }

    // Alinha para começar a varredura
    mover_tempo(-300, 239);
    objetivo_direita(converter_graus(eixo_x() + 90));
    preparar_atuador(true);
    chegar_final();
    // Após ir até o fim da direita, se alinha para começar o loop
    objetivo_esquerda(converter_graus(eixo_x() - 90));
    mover_tempo(-300, 300);
    objetivo_esquerda(converter_graus(eixo_x() - 90));

    // Loop para varrer os cantos
    for (int i = 0; i < 3; i++)
    {
        preparar_atuador(true);
        alinhar_angulo();
        chegar_final();
        mover_tempo(300, 191);
        alinhar_angulo();
        if (tem_vitima())
        {
            break;
        }
        if (luz(4) < 5)
        {
            alinhar_angulo();
            objetivo_direita(converter_graus(eixo_x() + 45));
            alinhar_ultra(35);
            mover_tempo(300, 239);
            objetivo_direita(converter_graus(eixo_x() + 20));
            mover_tempo(300, 191);
            objetivo_direita(converter_graus(eixo_x() + 25));
            mover_tempo(300, 639);
        }
        else
        {
            objetivo_direita(converter_graus(eixo_x() + 60));
            mover_tempo(300, 191);
            objetivo_direita(converter_graus(eixo_x() + 30));
        }
    }
}


// Variáveis de controle para ligar/desligar o debug e console
bool debug = false;
bool console = true;
bool registro = true;

// Método principal
void Main()
{
    if (debug)
    {
        levantar_atuador();
        alinhar_ultra(122);
        girar_direita(90);
        mover_tempo(-300, 300);
        passar_vassoura();
    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        bot.ActuatorSpeed(150);
        levantar_atuador();
        fechar_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            if (!pegou_kit && kit_frente())
            {
                parar();
                limpar_console();
                console_led(2, "<:KIT DE RESGATE IDENTIFICADO:>", "azul");
                while (kit_frente())
                {
                    mover(-250, -250);
                }
                parar();
                mover_tempo(-300, 239);
                abrir_atuador();
                girar_baixo_atuador();
                abaixar_atuador();
                int init_time = millis();
                timeout = millis() + 479;
                while (!tem_kit())
                {
                    mover(300, 300);
                    if (millis() > timeout)
                    {
                        print(1, "timeout");
                        break;
                    }
                }
                mover(300, 300);
                delay(192);
                fechar_atuador();
                girar_cima_atuador();
                levantar_atuador();
                parar();
                int kit_time = millis();
                mover_tempo(-300, (int)((kit_time - init_time) * 0.8));
                limpar_console();
                parar();
                if (tem_kit())
                {
                    pegou_kit = true;
                }
            }
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_fita_cinza();
        }
        limpar_console();
        print(2, "Sala de salvamento identificada");
        lugar = 2;
        alinhar_angulo();
        area_de_resgate();
        lugar = 3;
        while (lugar == 3)
        {
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
        }
        travar();
    }
}
