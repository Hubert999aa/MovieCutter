using Application.Interfaces;
using Domain.BusinessEnums;

namespace Application.Services
{
    public class OperationStatusManagerService : IOperationStatusManager
    {
        //


        public void UpdateOperationStatus(OperationStatus status)
        {
            // Aktualizacja statusu dla konkretnego video

            // Musimy tutaj sprawdzić jaki typ joba został odpalony dla danego video. W zależności od typu i złożoności będziemy mieli inny przelicznik % i inne procesy wyświetlane użytkownikowi

            // Po rozpoznaniu video, zmapowaniu postępu wysyłamy zmianę do SignalR i zapisujemy zmianę w bazie danych
        }

        public void UpdateOperationProgress(VideoProcess processType, int progress)
        {

        }
    }
}

/*
 * Notfikacje robimy per video
 * Ustawiamy różne stany - oczekuje, w toku, ukończono, błąd
 * Ustawiamy różne procesy - pobieranie, cięcie na kawałki, cięcie na klatki
 * Ustawiamy procent postepu w zależności od procesu i postępu w procesie - pobieranie zakres od 0-40, cięcie na kawałki 40-50, cięcie na klatki 50 - 100
 * Stworzyć serwis zarządzający stanem - poprzez ten serwis będzie chodzić cała komunikacja i tam będziemy zarządzać procentami etc.
 */