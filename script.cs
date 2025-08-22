// Verwende die Konfiguration aus config.js
const targetDate = new Date(CONFIG.targetDate);

// Globale Variable für die korrekte aktuelle Zeit
let currentTime = new Date();

// Verwende lokale Zeit (keine API-Aufrufe wegen CORS)
function getCurrentTime() {
    currentTime = new Date();
    console.log('Lokale Zeit verwendet:', currentTime.toLocaleString('de-DE'));
}

// Funktion zur automatischen Textgenerierung
function generateExpirationText() {
    const months = [
        'JANUARY', 'FEBRUARY', 'MARCH', 'APRIL', 'MAY', 'JUNE',
        'JULY', 'AUGUST', 'SEPTEMBER', 'OCTOBER', 'NOVEMBER', 'DECEMBER'
    ];
    
    // Verwende das Ziel-Datum direkt
    const month = months[targetDate.getMonth()];
    const day = targetDate.getDate();
    const hours = targetDate.getHours();
    const minutes = targetDate.getMinutes();
    
    // Formatiere die Zeit
    let timeString = '';
    if (hours === 0 && minutes === 0) {
        timeString = 'AT MIDNIGHT';
    } else if (hours === 12 && minutes === 0) {
        timeString = 'AT NOON';
    } else {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const displayHours = hours === 0 ? 12 : (hours > 12 ? hours - 12 : hours);
        const displayMinutes = minutes.toString().padStart(2, '0');
        timeString = `AT ${displayHours}:${displayMinutes} ${ampm}`;
    }
    
    // Zeitzone - verwende die Zeitzone aus der Konfiguration oder Standard
    let timezone = 'ET'; // Standard
    if (CONFIG.timezone) {
        if (CONFIG.timezone.includes('Europe/Berlin') || CONFIG.timezone.includes('Europe/Paris')) {
            timezone = 'CET';
        } else if (CONFIG.timezone.includes('America/New_York')) {
            timezone = 'ET';
        }
    } else {
        // Fallback: Prüfe die aktuelle lokale Zeitzone
        const now = new Date();
        const offset = now.getTimezoneOffset();
        if (offset === -60) {
            timezone = 'CET';
        } else if (offset === -120) {
            timezone = 'CEST';
        }
    }
    
    return {
        date: `EXPIRES ${month} ${day}`,
        time: `${timeString} ${timezone}`
    };
}

// Aktualisiere die Texte automatisch
document.addEventListener('DOMContentLoaded', function() {
    // Generiere automatische Texte
    const expirationText = generateExpirationText();
    
    // Verwende automatische Texte, falls keine in CONFIG gesetzt sind
    const expiresDate = CONFIG.expiresDate || expirationText.date;
    const expiresTime = CONFIG.expiresTime || expirationText.time;
    
    document.querySelector('.expires-date').textContent = expiresDate;
    document.querySelector('.expires-time').textContent = expiresTime;
    
    // Hole die lokale Zeit beim Laden
    getCurrentTime();
    
    // Zeige Debug-Informationen
    const timeDifference = targetDate - currentTime;
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
    
    console.log('Aktuelles Datum (API):', currentTime.toLocaleString('de-DE'));
    console.log('Ziel-Datum:', targetDate.toLocaleString('de-DE'));
    console.log('Verbleibende Tage:', days);
    console.log('Generierter Text:', expirationText);
    
    // Starte den Countdown
    updateCountdown();
});

function updateCountdown() {
    // Aktualisiere die lokale Zeit für jede Sekunde
    currentTime = new Date(currentTime.getTime() + 1000);
    
    const timeDifference = targetDate - currentTime;

    if (timeDifference <= 0) {
        // Countdown ist abgelaufen
        document.getElementById('countdown').textContent = '00:00:00:00';
        return;
    }

    // Berechne Tage, Stunden, Minuten und Sekunden
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
    const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((timeDifference % (1000 * 60)) / 1000);

    // Formatiere die Zeit mit führenden Nullen
    const formattedDays = days.toString().padStart(2, '0');
    const formattedHours = hours.toString().padStart(2, '0');
    const formattedMinutes = minutes.toString().padStart(2, '0');
    const formattedSeconds = seconds.toString().padStart(2, '0');

    // Aktualisiere die Anzeige (nur Stunden:Minuten:Sekunden wie im Original)
    document.getElementById('countdown').textContent = 
        `${formattedHours}:${formattedMinutes}:${formattedSeconds}`;
}

// Aktualisiere den Countdown basierend auf der Konfiguration
setInterval(updateCountdown, CONFIG.updateInterval);

// Aktualisiere die lokale Zeit alle 5 Minuten
setInterval(() => {
    getCurrentTime();
    console.log('Zeit aktualisiert:', currentTime.toLocaleString('de-DE'));
}, 5 * 60 * 1000); // Alle 5 Minuten 
