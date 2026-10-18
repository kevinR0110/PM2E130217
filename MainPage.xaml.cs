using System.Globalization;
namespace PM2E130217;
public partial class MainPage : ContentPage
{
    // ===== Tarifas por kilogramo y recargo (constantes, en lempiras) =====
    private const double TarifaLocal = 50.00;
    private const double TarifaNacional = 85.00;
    private const double TarifaInternacional = 150.00;
    private const double RecargoExpress = 100.00;
    // A partir de este monto la tarjeta cambia de color
    private const double LimiteMontoAlto = 300.00;
    // Colores de la tarjeta de resultados
    private static readonly Color ColorMontoBajo = Color.FromArgb("#DCFCE7"); // verde claro
    private static readonly Color ColorMontoAlto = Color.FromArgb("#E0F2FE"); // azul/gris claro
    public MainPage()
    {
        InitializeComponent();
    }
    // Evento del botón "Calcular envío": valida, calcula y muestra la tarjeta
    private async void OnCalcularClicked(object? sender, EventArgs e)
    {
        brdResultado.IsVisible = false; // oculta el resultado anterior
        // 1) Descripción vacía
        if (string.IsNullOrWhiteSpace(entDescripcion.Text))
        {
            await DisplayAlert("Falta la descripción", "Escribe la descripción del paquete.", "OK");
            return;
        }
        // 2) Peso vacío
        if (string.IsNullOrWhiteSpace(entPeso.Text))
        {
            await DisplayAlert("Falta el peso", "Escribe el peso del paquete en kilogramos.", "OK");
            return;
        }
        // 3) Peso numérico (acepta coma o punto; rechaza letras)
        string textoPeso = entPeso.Text.Trim().Replace(',', '.');
        if (!double.TryParse(textoPeso,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out double peso)
            || !double.IsFinite(peso))
        {
            await DisplayAlert("Peso inválido", "El peso debe ser un número, por ejemplo 2.5.", "OK");
            return;
        }
        // 4) Peso mayor que 0
        if (peso <= 0)
        {
            await DisplayAlert("Peso inválido", "El peso debe ser mayor que 0.", "OK");
            return;
        }
        // 5) Zona seleccionada (SelectedIndex = -1 significa "nada elegido")
        if (pickZona.SelectedIndex == -1)
        {
            await DisplayAlert("Falta la zona", "Selecciona la zona de destino.", "OK");
            return;
        }
        // Tarifa según la zona elegida (0 = local, 1 = nacional, 2 = internacional)
        double tarifa;
        switch (pickZona.SelectedIndex)
        {
            case 0:
                tarifa = TarifaLocal;
                break;
            case 1:
                tarifa = TarifaNacional;
                break;
            default:
                tarifa = TarifaInternacional;
                break;
        }
        // Recargo solo si el switch está activado
        double recargo = 0;
        if (swExpress.IsToggled)
        {
            recargo = RecargoExpress;
        }
        // Fórmula pedida: (Peso x Tarifa de zona) + Recargo exprés
        double subtotal = peso * tarifa;
        double total = subtotal + recargo;
        // Mostrar resultados
        lblTotal.Text = FormatoLempiras(total);
        lblPaquete.Text = $"Paquete: {entDescripcion.Text.Trim()}";
        lblDetalle.Text = $"{peso.ToString("0.##", CultureInfo.InvariantCulture)} kg x " +
                          $"{FormatoLempiras(tarifa)} = {FormatoLempiras(subtotal)}\n" +
                          $"Recargo exprés: {FormatoLempiras(recargo)}";
        // Color dinámico según el monto total
        if (total < LimiteMontoAlto)
        {
            brdResultado.BackgroundColor = ColorMontoBajo;
        }
        else
        {
            brdResultado.BackgroundColor = ColorMontoAlto;
        }
        brdResultado.IsVisible = true;
    }
    // Da formato de dinero con 2 decimales, por ejemplo: L350.00
    private static string FormatoLempiras(double valor)
    {
        return "L" + valor.ToString("F2", CultureInfo.InvariantCulture);
    }
}