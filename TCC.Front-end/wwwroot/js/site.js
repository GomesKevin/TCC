function exibirMensagem(mensagem, sucesso) {
    var elemento = $('#mensagem');
    elemento.text(mensagem);
    if (sucesso) {
        elemento.removeClass('alert-danger').addClass('alert-success');
    } else {
        elemento.removeClass('alert-success').addClass('alert-danger');
    }
    elemento.fadeIn();
    setTimeout(function () {
        elemento.fadeOut();
    }, 3000); // Tempo em milissegundos (3 segundos neste exemplo)
}

function adicionarItem(codigo, descricao, valor, caminhoImagem) {
    var model = {
        CodigoProduto: codigo,
        DescricaoProduto: descricao,
        ValorProduto: valor,
        CaminhoImagemProduto: caminhoImagem,
        Quantidade: 1
    };

    $.ajax({
        url: '/Home/AdicionarItem',
        type: 'POST',
        data: JSON.stringify(model),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                var qtdItensCarrinho = $('#qtdItensCarrinho');
                var qtdAtual = parseInt(qtdItensCarrinho.text());
                qtdItensCarrinho.text(qtdAtual + 1);

                exibirMensagem('Produto adicionado ao carrinho com sucesso!', true);
            } else {
                exibirMensagem('Ocorreu um erro ao adicionar o produto ao carrinho.', false);
            }
        },
        error: function () {
            exibirMensagem('Ocorreu um erro ao adicionar o produto ao carrinho.', false);
        }
    });
}


function removerItem(codigoItem) {
    var model = {
        CodigoProduto: codigoItem,
    };

    $.ajax({
        url: '/Home/RemoverItem',
        type: 'POST',
        data: JSON.stringify(model),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {

                var linhaRemover = $('.btn-remover[data-codigo="' + codigoItem + '"]').closest('tr');
                linhaRemover.remove();

                window.location.href = '/Carrinho/Index';

                exibirMensagem('Produto removido com sucesso!', true);
            } else {
                exibirMensagem('Ocorreu um erro ao remover o produto do carrinho.', false);
            }
        },
        error: function () {
            exibirMensagem('Ocorreu um erro ao remover o produto do carrinho.', false);
        }
    });
}
