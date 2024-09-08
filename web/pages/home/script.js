let isSubmitting = false;
const urlBase = 'https://apiazurerelacoespessoais-czdnemf2bwfxegda.brazilsouth-01.azurewebsites.net/'

async function load() {
    await this.obterListaPessoas();
}

async function obterListaPessoas() {
    try {
        const nome = document.getElementById('search-input').value;
        const response = await fetch(`${urlBase}api/pessoa/obter-lista-pessoas?nome=${nome}&page=${currentPage}`);

        if (!response.ok) throw new Error(`Erro: ${response.statusText}`);

        const { lista: pessoas, total } = await response.json();

        const tabela = document.querySelector('tbody');
        tabela.innerHTML = ''; // Limpa o conteúdo atual do tbody

        totalPages = total <= 9 ? 1 : Math.ceil(total / 9);
        totalRegistro = total;

        document.getElementById('pagination-container').style.display = total === 0 ? 'none' : '';

        if (pessoas.length > 0) {
            pessoas.forEach(pessoa => {
                const tr = document.createElement('tr');

                tr.innerHTML = `
                    <td class="icon-column">
                        <img src="../../imgs/user-solid.svg" class="icon-user" alt="Icone">
                    </td>
                    <td>${pessoa.nome}</td>
                    <td class="row-contato-dsc" title="${pessoa.contatosDsc}">${pessoa.contatosDsc}</td>
                    <td class="icon-column" style="padding-right: 35px;">
                        <img src="../../imgs/pen-to-square-regular.svg" class="icon" alt="Editar" onclick="mostrarModalRegistro(${pessoa.codPessoa})">
                        <img src="../../imgs/trash-can-regular.svg" class="icon" alt="Excluir" style="margin-left: 20px;" onclick="mostrarModalConfirmacao(${pessoa.codPessoa})">
                    </td>
                `;

                tabela.appendChild(tr);
            });

            document.getElementById('info-row').style.display = 'none';
            document.getElementById('tabela').style.display = '';
        } else {
            document.getElementById('info-row').style.display = '';
            document.getElementById('tabela').style.display = 'none';
        }

        updatePagination(); // Atualiza a paginação

    } catch (error) {
        mostrarModalErro('Não foi possível carregar os dados');
        console.error('Erro ao obter lista de pessoas:', error);
    }
}

function mostrarModalConfirmacao(codPessoa) {
    const modal = document.getElementById('confirmModal');
    const confirmarPessoa = document.getElementById('confirmDelete');
    const cancelar = document.getElementById('cancelDelete');

    // Exibe o modal
    modal.style.display = 'flex';

    // Remove os event listeners antigos
    confirmarPessoa.onclick = null;
    cancelar.onclick = null;

    // Adiciona novos event listeners
    confirmarPessoa.onclick = () => excluirPessoa(codPessoa);
    cancelar.onclick = () => modal.style.display = 'none';
}

async function excluirPessoa(codPessoa) {
    try {
        const response = await fetch(`${urlBase}api/pessoa/excluir-pessoa?codPessoa=${codPessoa}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            console.log('Pessoa excluída com sucesso.');
            fecharModalConfirmacao();
            await obterListaPessoas();
        } else {
            console.error('Erro ao excluir pessoa:', response.statusText);
        }
    } catch (error) {
        console.error('Erro na solicitação:', error);
    }
}

function fecharModalConfirmacao() {
    document.getElementById('confirmModal').style.display = 'none';
}


function aplicarMascara(selectElement) {
    const descricaoInputContainer = selectElement.closest('.tipo-contato-select-container').nextElementSibling;
    const descricaoInput = descricaoInputContainer.querySelector('.descricao-input');
    const tipoContato = selectElement.value;

    if (tipoContato === "1" || tipoContato === "3") {
        descricaoInput.setAttribute("placeholder", tipoContato === "1" ? "(xx)xxxx-xxxx" : "(xx)xxxxx-xxxx");
        descricaoInput.dataset.tipoContato = tipoContato;
        descricaoInput.addEventListener('input', formatarTelefone);
    } else if (tipoContato === "2") {
        descricaoInput.setAttribute("placeholder", "email@example.com");
        descricaoInput.removeAttribute("data-tipo-contato");
        descricaoInput.removeEventListener('input', formatarTelefone);
    } else {
        descricaoInput.setAttribute("placeholder", ""); // Limpa o placeholder se não houver opção
        descricaoInput.removeAttribute("data-tipo-contato");
        descricaoInput.removeEventListener('input', formatarTelefone);
    }
}

function formularioValido() {
    const contatos = document.querySelectorAll(".contato-row");
    let formularioInvalido = false;
    const nomeVazio = document.getElementById("nome-input").value.length == 0;

    if (nomeVazio) return false; // Se o nome estiver vazio, o formulário é inválido

    contatos.forEach(contato => {
        const tipoContato = contato.querySelector(".tipo-contato").value;
        const valorContato = contato.querySelector(".descricao-input").value;
        let regex;

        if (tipoContato === "0") {
            formularioInvalido = true;
        }

        if (tipoContato === "1") { // Telefone
            regex = /^\(\d{2}\)\d{4}-\d{4}$/; // Regex para telefone
        } else if (tipoContato === "3") { // WhatsApp
            regex = /^\(\d{2}\)\d{5}-\d{4}$/; // Regex para WhatsApp
        } else if (tipoContato === "2") { // E-mail
            regex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/; // Regex para e-mail
        }

        if (regex && !regex.test(valorContato)) {
            formularioInvalido = true;
        }
    });

    return !formularioInvalido; // Retorna se o formulário está válido
}

function formatarTelefone(event) {
    let input = event.target;
    let value = input.value.replace(/\D/g, ''); // Remove caracteres não numéricos

    // Limita o valor a 13 caracteres para telefone e 14 para WhatsApp
    if (input.dataset.tipoContato === "1") { // Telefone
        value = value.substring(0, 10); // 10 dígitos para telefone
        value = value.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1)$2-$3');
    } else if (input.dataset.tipoContato === "3") { // WhatsApp
        value = value.substring(0, 11); // 11 dígitos para WhatsApp
        value = value.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1)$2-$3');
    }

    input.value = value;
}

//#region Contato
function adicionarContato(contato) {
    const container = document.getElementById("contatosContainer");
    const contatoRow = document.createElement("div");
    contatoRow.className = "contato-row";

    contatoRow.innerHTML = `
            <input type="hidden" class="codContato" id="codContato" value="${contato != null ? contato.codContato : 0}">
            <div class="tipo-contato-select-container">
                <label for="tipo-contato" class="lbl-tipo-contato">Tipo de Contato: </label>
                <select class="tipo-contato" onchange="aplicarMascara(this)">
                    <option value="0"></option>
                    <option value="1" ${contato != null && contato.tipoContato == 1 ? ' selected' : ''}>Telefone</option>
                    <option value="2" ${contato != null && contato.tipoContato == 2 ? ' selected' : ''}>E-mail</option>
                    <option value="3" ${contato != null && contato.tipoContato == 3 ? ' selected' : ''}>Whatsapp</option>
                </select>
            </div>
    
            <div class="d-flex" style="padding: 0;">
                <label for="descricao" class="lbl-descricao">Descrição: </label>
                <div class="descricao-input-container">
                    <input type="text" class="descricao-input" maxlength="100" ${contato != null ? 'value=' + contato.valorContato : ''}>
                </div>
            </div>
    
            <button class="delete-contato" onclick="removerContato(this)">
                <img src="../../imgs/trash-can-regular.svg" class="icon" style="height: 24px;" alt="Excluir">
            </button>
        `;

    container.appendChild(contatoRow);
}

async function removerContato(buttonElement) {
    const codigoContato = Number(buttonElement.closest(".contato-row").querySelector(".codContato").value);

    if (codigoContato !== 0) {
        mostrarModalExcluirContato(codigoContato, buttonElement);
    } else {
        removerContatoLocal(buttonElement);
    }
}

function mostrarModalExcluirContato(codigoContato, buttonElement) {
    const modal = document.getElementById('confirmModalContato');
    modal.style.display = 'flex';

    const confirmarContato = document.getElementById('confirmDeleteContato');
    confirmarContato.onclick = () => excluirContato(buttonElement, codigoContato);

    document.getElementById('cancelDeleteContato').onclick = () => fecharModal(modal);
}

function fecharModal(modal) {
    modal.style.display = 'none';
}

function removerContatoLocal(buttonElement) {
    buttonElement.closest(".contato-row").remove();
}

async function excluirContato(buttonElement, codigoContato) {
    if (isSubmitting) return;
    isSubmitting = true;

    try {
        const response = await fetch(`${urlBase}api/contato/excluir-contato?codContato=${codigoContato}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            removerContatoLocal(buttonElement);
            fecharModal(document.getElementById('confirmModalContato'));
            await obterListaPessoas(); // Atualiza a lista após exclusão
        }
    } catch (error) {
        console.error('Erro ao excluir contato:', error);
    } finally {
        isSubmitting = false;
    }
}

//#endregion

//#region Salvar e Atualizar

function mostrarModalRegistro(codigo) {
    const modal = document.getElementById('registroModal');
    const titulo = document.getElementById("titulo-head-modal");
    const btnConfirmar = document.getElementById('confirmarRegistro');

    modal.style.display = 'flex';
    document.getElementById("nome-input").value = '';
    document.getElementById("contatosContainer").innerHTML = '';

    if (codigo === 0) {
        titulo.textContent = 'Incluir Registro';
        adicionarContato();
        btnConfirmar.onclick = salvar;
    } else {
        titulo.textContent = 'Atualizar Registro';
        atualizarRegistro(codigo);
        btnConfirmar.onclick = atualizar;
    }
}

function fecharModalRegistro() {
    document.getElementById('registroModal').style.display = 'none';
}

async function atualizarRegistro(codigo) {
    if (isSubmitting) return;
    isSubmitting = true;

    const botaoSalvar = document.getElementById("confirmarRegistro");
    botaoSalvar.disabled = true;

    try {
        const response = await fetch(`${urlBase}api/pessoa/obter-pessoa?codPessoa=${codigo}`);
        if (!response.ok) throw new Error(`Erro: ${response.statusText}`);

        const { codPessoa, nome, contatos } = await response.json();

        document.getElementById("codPessoa").value = codPessoa;
        document.getElementById("nome-input").value = nome;
        contatos.forEach(adicionarContato);

    } catch (error) {
        console.error(error);
    } finally {
        isSubmitting = false;
        botaoSalvar.disabled = false;
    }
}

async function atualizar() {
    if (isSubmitting) return;
    isSubmitting = true;

    const botaoSalvar = document.getElementById("confirmarRegistro");
    botaoSalvar.disabled = true;

    try {
        if (!formularioValido()) {
            const nomeVazio = document.getElementById("nome-input").value.length === 0;
            return mostrarModalErro(
                `Não foi possível atualizar os registros.<br> Existem campos inválidos, verifique e tente novamente.<br> ${nomeVazio ? 'Nome deve ser preenchido.' : 'Os contatos devem seguir o formato informado no campo.'}`
            );
        }

        const codPessoa = document.getElementById("codPessoa").value;
        const nome = document.getElementById("nome-input").value;

        const objPessoa = { codPessoa, nome };
        const responsePessoa = await fetch(`${urlBase}api/pessoa/editar-pessoa`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(objPessoa)
        });

        if (!responsePessoa.ok) throw new Error(`Erro ao editar pessoa: ${responsePessoa.statusText}`);

        const listaContato = Array.from(document.querySelectorAll(".contato-row")).map(contato => ({
            codPessoa,
            codContato: contato.querySelector(".codContato").value,
            tipoContato: contato.querySelector(".tipo-contato").value,
            valorContato: contato.querySelector(".descricao-input").value
        }));

        const responseContato = await fetch(`${urlBase}api/contato/editar-contato`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(listaContato)
        });

        if (!responseContato.ok) throw new Error(`Erro ao editar contato: ${responseContato.statusText}`);

        fecharModalRegistro();
        await obterListaPessoas();
        return await responseContato.json();

    } catch (error) {
        console.error(error);
    } finally {
        isSubmitting = false;
        botaoSalvar.disabled = false;
    }
}

async function salvar() {
    if (isSubmitting) return;
    isSubmitting = true;

    const botaoSalvar = document.getElementById("confirmarRegistro");
    botaoSalvar.disabled = true;

    try {
        if (!formularioValido()) {
            const nomeVazio = document.getElementById("nome-input").value.length === 0;
            const mensagemErro = `Não foi possível atualizar os registros. <br> Existem campos inválidos, verifique e tente novamente. <br> ${nomeVazio ? 'Nome deve ser preenchido, os' : 'Os'} contatos devem seguir o formato informado no campo.`;
            return mostrarModalErro(mensagemErro);
        }

        const nome = document.getElementById("nome-input").value;
        const objPessoa = { nome };

        const responsePessoa = await fetch(`${urlBase}api/pessoa/incluir-pessoa`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(objPessoa)
        });

        if (!responsePessoa.ok) throw new Error(`Erro ao incluir pessoa: ${responsePessoa.statusText}`);

        const dadosPessoa = await responsePessoa.json();
        const listaContato = Array.from(document.querySelectorAll(".contato-row")).map(contato => ({
            codPessoa: dadosPessoa.codPessoa,
            tipoContato: contato.querySelector(".tipo-contato").value,
            valorContato: contato.querySelector(".descricao-input").value
        }));

        const responseContato = await fetch(`${urlBase}api/contato/incluir-contato`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(listaContato)
        });

        if (!responseContato.ok) throw new Error(`Erro ao incluir contato: ${responseContato.statusText}`);

        fecharModalRegistro();
        await obterListaPessoas();

        return await responseContato.json();
    } catch (error) {
        console.error(error.message);
    } finally {
        isSubmitting = false;
        botaoSalvar.disabled = false;
    }
}

//#endregion

function mostrarModalErro(mensagem) {
    const modal = document.getElementById('errorModal');
    modal.style.display = 'flex';
    document.getElementById('titulo-erro-modal').innerHTML = mensagem;

    var cancelar = document.getElementById('accept-btn');
    cancelar.removeEventListener('click', () => modal.style.display = 'none');
    cancelar.addEventListener('click', () => modal.style.display = 'none');
}


//#region Paginação
const totalPagesDsc = document.getElementById('total-pages');
const totalRegistroDsc = document.getElementById('total-registro');
const paginationElement = document.getElementById('pagination');
const pageNumbersElement = document.getElementById('page-numbers');
const firstPageButton = document.getElementById('first-page');
const prevPageButton = document.getElementById('prev-page');
const nextPageButton = document.getElementById('next-page');
const lastPageButton = document.getElementById('last-page');

var totalPages = 0;
var totalRegistro = 0;
var currentPage = 1;
const pagesToShow = 3;

function updatePagination() {
    totalPagesDsc.innerHTML = totalPages;
    totalRegistroDsc.innerHTML = totalRegistro;
    pageNumbersElement.innerHTML = '';
    const halfPagesToShow = Math.floor(pagesToShow / 2);
    let startPage = Math.max(1, currentPage - halfPagesToShow);
    let endPage = Math.min(totalPages, currentPage + halfPagesToShow);

    // Adjust the range if there are not enough pages on one side
    if (currentPage - halfPagesToShow < 1) {
        endPage = Math.min(totalPages, endPage + (1 - (currentPage - halfPagesToShow)));
    } else if (currentPage + halfPagesToShow > totalPages) {
        startPage = Math.max(1, startPage - ((currentPage + halfPagesToShow) - totalPages));
    }

    firstPageButton.disabled = currentPage === 1;
    prevPageButton.disabled = currentPage === 1;
    nextPageButton.disabled = currentPage === totalPages;
    lastPageButton.disabled = currentPage === totalPages;

    for (let i = startPage; i <= endPage; i++) {
        pageNumbersElement.appendChild(createPageButton(i, i === currentPage));
    }
}

function createPageButton(pageNumber, isActive = false) {
    const button = document.createElement('button');
    button.textContent = pageNumber;
    button.classList.toggle('active', isActive);
    button.addEventListener('click', () => goToPage(pageNumber));
    return button;
}

function goToPage(pageNumber) {
    if (pageNumber < 1 || pageNumber > totalPages) return;
    currentPage = pageNumber;
    updatePagination();
    fetchPageData(currentPage);
}

function fetchPageData(pageNumber) {
    console.log(`Fetching data for page ${pageNumber}`);
    obterListaPessoas()
}

function firstPage() { goToPage(1); }

function previousPage() { goToPage(currentPage - 1); }

function nextPage() { goToPage(currentPage + 1); }

function lastPage() { goToPage(totalPages); }

//#endregion



window.onload = load;