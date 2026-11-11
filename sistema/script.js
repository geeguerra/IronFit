const API = "http://localhost:5091/api";

function mostrarTela(id) {
    document.querySelectorAll("main section").forEach(tela => {
        tela.style.display = "none";
    });

    document.getElementById(id).style.display = "block";

    if (id === "telaAlunos") {
        carregarPersonals();
        listarAlunos();
    }
}

document.getElementById("formLogin").addEventListener("submit", async function(event) {
    event.preventDefault();

    const mensagem = document.getElementById("mensagemLogin");

    try {
        const resposta = await fetch(API + "/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: document.getElementById("username").value,
                password: document.getElementById("password").value
            })
        });

        if (!resposta.ok) {
            mensagem.textContent = "Usuário ou senha inválidos!";
            return;
        }

        const dados = await resposta.json();

        localStorage.setItem("token", dados.token || dados.Token);
        localStorage.setItem("nome", dados.nome || dados.Nome);

        document.getElementById("nomeUsuario").textContent =
            "Bem-vindo, " + localStorage.getItem("nome");

        document.getElementById("btnSair").style.display = "block";

        mostrarTela("painel");

    } catch (erro) {
        mensagem.textContent = "Erro ao conectar com a API!";
        console.log(erro);
    }
});

function sair() {
    localStorage.clear();
    location.reload();
}

async function carregarPersonals() {
    try {
        const resposta = await fetch(API + "/Personals");
        const personals = await resposta.json();

        const select = document.getElementById("personalAluno");
        select.innerHTML = "";

        personals.forEach(p => {
            select.innerHTML += `
                <option value="${p.id}">${p.nome}</option>
            `;
        });
    } catch (erro) {
        console.log("Erro ao carregar Personals", erro);
    }
}

async function listarAlunos() {
    try {
        const resposta = await fetch(API + "/Alunos");
        const alunos = await resposta.json();

        const lista = document.getElementById("listaAlunos");
        lista.innerHTML = "";

        alunos.forEach(a => {
            lista.innerHTML += `
                <div class="item">
                    Nome: ${a.nome}<br>
                    Personal: ${a.nomePersonal}
                </div>
            `;
        });
    } catch (erro) {
        console.log("Erro ao listar alunos", erro);
    }
}

document.getElementById("formAluno").addEventListener("submit", async function(event) {
    event.preventDefault();

    try {
        const resposta = await fetch(API + "/Alunos", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                nome: document.getElementById("nomeAluno").value,
                cpf: document.getElementById("cpfAluno").value,
                personalId: Number(document.getElementById("personalAluno").value)
            })
        });

        const mensagem = document.getElementById("mensagemAluno");

        if (resposta.ok) {
            mensagem.textContent = "Aluno cadastrado com sucesso!";
            this.reset();
            listarAlunos();
        } else {
            mensagem.textContent = "Erro ao cadastrar aluno!";
        }
    } catch (erro) {
        document.getElementById("mensagemAluno").textContent =
            "Erro ao conectar com a API!";
    }
});

let personalsCarregados = [];

async function listarPersonals() {
    try {
        const busca = document.getElementById("buscaPersonal").value;

        const resposta = await fetch(
            API + "/Personals?busca=" + encodeURIComponent(busca)
        );

        personalsCarregados = await resposta.json();

        const lista = document.getElementById("listaPersonals");
        lista.innerHTML = "";

        personalsCarregados.forEach(p => {
            lista.innerHTML += `
                <div class="item">
                    ${p.nome}
                    <button onclick="editarPersonal(${p.id})">Editar</button>
                </div>
            `;
        });
    } catch (erro) {
        console.log("Erro ao listar Personals", erro);
    }
}

function editarPersonal(id) {
    const personal = personalsCarregados.find(p => p.id === id);

    if (personal) {
        document.getElementById("idPersonal").value = personal.id;
        document.getElementById("nomePersonal").value = personal.nome;
        document.getElementById("cpfPersonal").value = personal.cpf;
    }
}

document.getElementById("formPersonal").addEventListener("submit", async function(event) {
    event.preventDefault();

    try {
        const id = document.getElementById("idPersonal").value;

        const resposta = await fetch(API + "/Personals/" + id, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                nome: document.getElementById("nomePersonal").value,
                cpf: document.getElementById("cpfPersonal").value
            })
        });

        const mensagem = document.getElementById("mensagemPersonal");

        if (resposta.ok) {
            mensagem.textContent = "Personal atualizado com sucesso!";
            this.reset();
            listarPersonals();
        } else {
            mensagem.textContent = "Erro ao atualizar Personal!";
        }
    } catch (erro) {
        document.getElementById("mensagemPersonal").textContent =
            "Erro ao conectar com a API!";
    }
});

async function listarAgendamentos() {
    try {
        const resposta = await fetch(API + "/Agendamentos");
        const agendamentos = await resposta.json();

        const lista = document.getElementById("listaAgendamentos");
        lista.innerHTML = "";

        agendamentos.forEach(a => {
            lista.innerHTML += `
                <div class="item">
                    <strong>Agendamento ${a.id}</strong><br>
                    Aluno: ${a.nomeAluno}<br>
                    Personal: ${a.nomePersonal}<br>
                    Data: ${new Date(a.dataHora).toLocaleString("pt-BR")}
                </div>
            `;
        });
    } catch (erro) {
        console.log("Erro ao listar agendamentos", erro);
    }
}

document.getElementById("formAgendamento").addEventListener("submit", async function(event) {
    event.preventDefault();

    try {
        const id = document.getElementById("idAgendamento").value;
        const data = document.getElementById("dataAgendamento").value;

        const resposta = await fetch(API + "/Agendamentos/" + id, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                dataHora: data
            })
        });

        const mensagem = document.getElementById("mensagemAgendamento");

        if (resposta.ok) {
            mensagem.textContent = "Agendamento atualizado!";
            this.reset();
            listarAgendamentos();
        } else {
            mensagem.textContent = "Erro ao atualizar agendamento!";
        }
    } catch (erro) {
        document.getElementById("mensagemAgendamento").textContent =
            "Erro ao conectar com a API!";
    }
});
