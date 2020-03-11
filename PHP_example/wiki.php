<?php
//Pošlje POST na API in vrne na novo prejeto sporočilo
$url = 'https://www.dev.nikigre.si/sms/api.php';
$data = array('func' => '0001', 'user' => 'wiki123');


$options = array(
    'http' => array(
        'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
        'method'  => 'POST',
        'content' => http_build_query($data)
    )
);

$context  = stream_context_create($options);
$result = file_get_contents($url, false, $context);

if($result != "NO RECEIVED SMS")
{
    //Iz rezultata dobi telefonsko številko in iskanje in ju shrani
$polje= explode("'",$result);

$stevilka= $polje[3];
$iskanje= $polje[5];

//Preveri, če slučajno uporabnik potrebuje pomoč pri iskanju in če jo, mu pošlje podatke kako iskati.
if($iskanje=="pomoč" || $iskanje=="pomoc" || $iskanje=="POMOČ" || $iskanje=="POMOC" || $iskanje=="help" || $iskanje=="Help" || $iskanje=="HELP")
{
    PosljiSMS("Pozdravljeni!\nZa iskanje po slovenski Wikipediji, pošljite SMS z ključno besedo wiki [iskani izraz].\nIskani izraz lahko vsebuje šumnike.\nPrimer: wiki Slovenija.\nSistem išče samo po naslovih člankov in ne po vsebini!\nLep pozdrav", $stevilka);
}
else{

    //Pošlje GET request na wikipedia API in priidobi JSON podatkov
    $odgovor = file_get_contents("https://sl.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles=" . urlencode($iskanje));

    //V spremenljivko shani JSON obliko v asociative array
    $json = json_decode($odgovor, true);

    //Pridobi številko članka
    $key = array_keys($json['query']['pages']); 

    //Si shrani extract, po JSON strukturi extract članka
    $odgovor=$json['query']['pages'][$key[0]]['extract'];
    
    //Če je odgovor daljši od 3 znakov ga pošlje uporabniku. Če ne, pošlje da članka ni našlo
    if(strlen($odgovor)>2)
    {
        PosljiSMS($odgovor, $stevilka);
    }
    else{
        PosljiSMS("Odgovora ne najdem!", $stevilka);
    }
}
}
else{
    echo "No Info received";
}
//Pošlje SMS na določeno številko in vrne OK če je poslano
function PosljiSMS($sms, $tel)
{
    $url = 'https://www.dev.nikigre.si/sms/api.php';
    $data = array('func' => '10000', 'user' => 'wiki123', 'message' => $sms, 'phone' => $tel);


    $options = array(
        'http' => array(
            'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
            'method'  => 'POST',
            'content' => http_build_query($data)
        )
    );
    $context  = stream_context_create($options);
    $result = file_get_contents($url, false, $context);
    if ($result === FALSE) { echo "Error"; } else { echo "OK"; }
}

?>