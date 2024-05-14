var _alphabetSearch = '';
 
$.fn.dataTable.ext.search.push( function ( settings, searchData ) {
    if ( ! _alphabetSearch ) {
        return true;
    }
 
    if ( searchData[0].charAt(0) === _alphabetSearch ) {
        return true;
    }
 
    return false;
} );
 
 
function bin ( data ) {
    var letter, bins = {};
 
    for ( var i=0, ien=data.length ; i<ien ; i++ ) {
        letter = data[i].charAt(0).toUpperCase();
 
        if ( bins[letter] ) {
            bins[letter]++;
        }
        else {
            bins[letter] = 1;
        }
    }
 
    return bins;
}
 
 
$(document).ready(function () {
    var table = $('#example').dataTable;
 
    var alphabet = $('<div class="alphabet"/>').append( 'Search: ' );
    var columnData = table.column(0).data();
    var bins = bin( columnData );
 
    $('<span class="clear active"/>')
        .data( 'letter', '' )
        .data( 'match-count', columnData.length )
        .html( 'None' )
        .appendTo( alphabet );
 
    for ( var i=0 ; i<26 ; i++ ) {
        var letter = String.fromCharCode( 65 + i );
 
        $('<span/>')
            .data( 'letter', letter )
            .data( 'match-count', bins[letter] || 0 )
            .addClass( ! bins[letter] ? 'empty' : '' )
            .html( letter )
            .appendTo( alphabet );
    }
 
    alphabet.insertBefore( table.table().container() );
 
    alphabet.on( 'click', 'span', function () {
        alphabet.find( '.active' ).removeClass( 'active' );
        $(this).addClass( 'active' );
 
        _alphabetSearch = $(this).data('letter');
        table.draw();
    } );
 
    var info = $('<div class="alphabetInfo"></div>')
        .appendTo( alphabet );
 
    alphabet
        .on( 'mouseenter', 'span', function () {
            info
                .css( {
                    opacity: 1,
                    left: $(this).position().left,
                    width: $(this).width()
                } )
                .html( $(this).data('match-count') )
        } )
        .on( 'mouseleave', 'span', function () {
            info.css('opacity', 0);
        } );
} );