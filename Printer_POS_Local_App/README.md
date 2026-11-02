# POS Print Service

## Description

Local Windows Service responsible for printing booking tickets on USB thermal printers.

---

## Technology

- .NET 8
- ASP.NET Core Web API
- Windows Service
- ESC/POS
- SkiaSharp

---

## Default URL

http://127.0.0.1:5005

---

## Requirements

- Windows 10/11
- XPrinter Driver Installed
- .NET 8 Runtime (if not self-contained)

---

## Installation

Copy publish folder:

C:\POSPrintService

Create service:

```cmd
sc create POSPrintService ^
binPath= "C:\POSPrintService\Printer_POS_Local_App.exe" ^
start= auto
```

Start service

```cmd
sc start POSPrintService
```

---

## Remove Service

```cmd
sc stop POSPrintService
sc delete POSPrintService
```

---

## Endpoints

POST /api/PrintTicket

Print booking ticket.
