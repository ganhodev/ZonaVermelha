namespace ZonaVermelha.Domain;

public static class CalculadoraDistancia
{
    public static double CalcularDistanciaMetros(double lat1, double lng1, double lat2, double lng2)
    {
        double lng1Rad = lng1 * (Math.PI / 180);
       double lat1Rad = lat1 * (Math.PI / 180);

        double lng2Rad = lng2 * (Math.PI / 180);
        double lat2Rad = lat2 * (Math.PI / 180);

        var diferencaLat = lat2Rad - lat1Rad;
        var diferencaLng = lng2Rad - lng1Rad;

        var senoMetadeLat = Math.Sin(diferencaLat / 2);
        var senoMetadeLng = Math.Sin(diferencaLng / 2);

        var a = (senoMetadeLat * senoMetadeLat)
              + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * (senoMetadeLng * senoMetadeLng);

        // Fórmula ângulo central
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double raioTerraMetros = 6371000; // Raio médio da Terra em metros

        var distanciaMetros = raioTerraMetros * c;

        return distanciaMetros;
    }
}
