bool debug = false;

calibrar();


while(!debug){
    verifica_calibrar();
    seguir_linha();
    verifica_curva();
}


while(debug){
    print(1, $"{tem_azul(0)} | {tem_azul(1)} | {tem_azul(2)} | {tem_azul(3)}");
}
