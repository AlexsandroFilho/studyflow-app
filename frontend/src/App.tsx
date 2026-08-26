import { useState, useMemo } from "react";
import { Header, ViewMode } from "./components/layout/Header";
import { Sidebar } from "./components/layout/Sidebar";
import { CanvasBoard } from "./components/canvas/CanvasBoard";
import { ObsidianEditor } from "./components/editor/ObsidianEditor";
import { CreateNotaModal } from "./components/modals/CreateNotaModal";
import { CreateTemaModal } from "./components/modals/CreateTemaModal";
import { ConfirmDeleteModal } from "./components/modals/ConfirmDeleteModal";
import { Spinner } from "./components/ui/Spinner";
import { useTemas } from "./hooks/useTemas";
import { useNotas } from "./hooks/useNotas";
import { useConexoes } from "./hooks/useConexoes";
import { useCanvas } from "./hooks/useCanvas";
import { Tema } from "./types/tema";
import { Nota } from "./types/nota";
import { CanvasNode, CanvasEdge } from "./types/canvas";

export function App() {
  const [viewMode, setViewMode] = useState<ViewMode>("canvas");
  const [searchTerm, setSearchTerm] = useState<string>("");

  // Modais
  const [isNotaModalOpen, setIsNotaModalOpen] = useState(false);
  const [isTemaModalOpen, setIsTemaModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

  // Estados de edição / deleção / conexão
  const [editingNota, setEditingNota] = useState<Nota | null>(null);
  const [connectedSourceNota, setConnectedSourceNota] = useState<Nota | null>(null);
  const [editingTema, setEditingTema] = useState<Tema | null>(null);
  const [itemToDelete, setItemToDelete] = useState<{
    type: "tema" | "nota" | "conexao";
    id: number;
    title: string;
  } | null>(null);

  // Custom Hooks
  const {
    temas,
    selectedTemaId,
    setSelectedTemaId,
    loading: loadingTemas,
    criarTema,
    atualizarTema,
    deletarTema,
  } = useTemas();

  const {
    notas,
    activeNota,
    setActiveNota,
    loading: loadingNotas,
    carregarNotas,
    criarNota,
    atualizarNota,
    atualizarParcialNota,
    deletarNota,
  } = useNotas(selectedTemaId);

  const {
    conexoes,
    carregarConexoes,
    conectar,
    criarNotaConectada,
    desconectarPorId,
  } = useConexoes(selectedTemaId);

  // Filtro de busca
  const filteredNotas = useMemo(() => {
    if (!searchTerm.trim()) return notas;
    const term = searchTerm.toLowerCase();
    return notas.filter(
      (n) =>
        n.titulo.toLowerCase().includes(term) ||
        n.conteudo.toLowerCase().includes(term)
    );
  }, [notas, searchTerm]);

  // Hook do Canvas
  const {
    nodes,
    edges,
    viewport,
    connectingSourceId,
    connectingMousePos,
    handleCanvasMouseDown,
    handleNodeMouseDown,
    handleStartConnecting,
    handleMouseMove,
    handleMouseUp,
    zoomIn,
    zoomOut,
    resetView,
  } = useCanvas(filteredNotas, conexoes, async (sourceId, targetId) => {
    try {
      await conectar({
        nota_origem_id: sourceId,
        nota_destino_id: targetId,
      });
    } catch (err: any) {
      console.error(err);
    }
  });

  // Handlers para Navegação e Ações
  const handleOpenInEditor = (notaId: number) => {
    const nota = notas.find((n) => n.id === notaId);
    if (nota) {
      setActiveNota(nota);
      setViewMode("editor");
    }
  };

  const handleEditNotaFromCanvas = (node: CanvasNode) => {
    setEditingNota(node.data);
    setConnectedSourceNota(null);
    setIsNotaModalOpen(true);
  };

  const handleCreateConnectedNote = (sourceNode: CanvasNode) => {
    setEditingNota(null);
    setConnectedSourceNota(sourceNode.data);
    setIsNotaModalOpen(true);
  };

  const handleDeleteEdgePrompt = (edge: CanvasEdge) => {
    const edgeId = parseInt(edge.id.replace("edge-", ""), 10);
    setItemToDelete({
      type: "conexao",
      id: edgeId,
      title: "a linha de conexão entre estas duas notas",
    });
    setIsDeleteModalOpen(true);
  };

  const handleDeleteNotaPrompt = (nodeOrNota: CanvasNode | Nota) => {
    const nota = "data" in nodeOrNota ? nodeOrNota.data : nodeOrNota;
    setItemToDelete({
      type: "nota",
      id: nota.id,
      title: `a nota "${nota.titulo}"`,
    });
    setIsDeleteModalOpen(true);
  };

  const handleDeleteTemaPrompt = (tema: Tema) => {
    setItemToDelete({
      type: "tema",
      id: tema.id,
      title: `o tema "${tema.nome}" e todas as suas notas associadas`,
    });
    setIsDeleteModalOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!itemToDelete) return;
    if (itemToDelete.type === "nota") {
      await deletarNota(itemToDelete.id);
      await carregarConexoes();
      if (activeNota?.id === itemToDelete.id) {
        setViewMode("canvas");
      }
    } else if (itemToDelete.type === "conexao") {
      await desconectarPorId(itemToDelete.id);
    } else {
      await deletarTema(itemToDelete.id);
      await carregarConexoes();
    }
  };

  return (
    <div className="flex flex-col h-screen w-screen bg-[#161B22] text-[#DDE6ED] overflow-hidden font-sans select-none">
      {/* Header Global */}
      <Header
        viewMode={viewMode}
        setViewMode={setViewMode}
        onOpenCreateNota={() => {
          setEditingNota(null);
          setConnectedSourceNota(null);
          setIsNotaModalOpen(true);
        }}
        onOpenCreateTema={() => {
          setEditingTema(null);
          setIsTemaModalOpen(true);
        }}
        totalNotas={notas.length}
        totalTemas={temas.length}
        totalConexoes={conexoes.length}
      />

      {/* Corpo Principal (Sidebar + Canvas / Editor) */}
      <div className="flex-1 flex overflow-hidden">
        {/* Sidebar Esquerda */}
        <Sidebar
          temas={temas}
          notas={notas}
          selectedTemaId={selectedTemaId}
          onSelectTema={setSelectedTemaId}
          onOpenCreateTema={() => {
            setEditingTema(null);
            setIsTemaModalOpen(true);
          }}
          onEditTema={(tema) => {
            setEditingTema(tema);
            setIsTemaModalOpen(true);
          }}
          onDeleteTema={handleDeleteTemaPrompt}
          searchTerm={searchTerm}
          onSearchChange={setSearchTerm}
        />

        {/* Área de Trabalho */}
        <main className="flex-1 flex relative overflow-hidden bg-[#161B22]">
          {loadingTemas || loadingNotas ? (
            <div className="flex-1 flex items-center justify-center bg-[#161B22]">
              <Spinner size="lg" />
            </div>
          ) : viewMode === "canvas" ? (
            <CanvasBoard
              nodes={nodes}
              edges={edges}
              temas={temas}
              viewport={viewport}
              connectingSourceId={connectingSourceId}
              connectingMousePos={connectingMousePos}
              onCanvasMouseDown={handleCanvasMouseDown}
              onNodeMouseDown={handleNodeMouseDown}
              onStartConnecting={handleStartConnecting}
              onMouseMove={handleMouseMove}
              onMouseUp={handleMouseUp}
              onOpenInEditor={handleOpenInEditor}
              onEditNota={handleEditNotaFromCanvas}
              onDeleteNota={handleDeleteNotaPrompt}
              onCreateConnectedNote={handleCreateConnectedNote}
              onDeleteEdge={handleDeleteEdgePrompt}
              onZoomIn={zoomIn}
              onZoomOut={zoomOut}
              onResetView={resetView}
              onOpenCreateNota={() => {
                setEditingNota(null);
                setConnectedSourceNota(null);
                setIsNotaModalOpen(true);
              }}
            />
          ) : activeNota ? (
            <ObsidianEditor
              nota={activeNota}
              temas={temas}
              onUpdateNota={atualizarParcialNota}
              onDeleteNota={handleDeleteNotaPrompt}
              onBackToCanvas={() => setViewMode("canvas")}
            />
          ) : (
            <div className="flex-1 flex items-center justify-center text-xs text-[#9DB2BF]">
              Nenhuma nota selecionada para edição. Selecione uma nota no Mapa Mental ou crie uma nova.
            </div>
          )}
        </main>
      </div>

      {/* Modais */}
      <CreateNotaModal
        isOpen={isNotaModalOpen}
        onClose={() => {
          setIsNotaModalOpen(false);
          setConnectedSourceNota(null);
        }}
        temas={temas}
        selectedTemaId={selectedTemaId}
        editingNota={editingNota}
        connectedSourceNota={connectedSourceNota}
        onSubmitCreate={async (dto) => {
          await criarNota(dto);
        }}
        onSubmitUpdate={async (id, dto) => {
          await atualizarNota(id, dto);
        }}
        onSubmitCreateConnected={async (dto) => {
          await criarNotaConectada(dto);
          await carregarNotas();
        }}
      />

      <CreateTemaModal
        isOpen={isTemaModalOpen}
        onClose={() => setIsTemaModalOpen(false)}
        editingTema={editingTema}
        onSubmitCreate={async (dto) => {
          await criarTema(dto);
        }}
        onSubmitUpdate={async (id, dto) => {
          await atualizarTema(id, dto);
        }}
      />

      <ConfirmDeleteModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        title="Confirmar Exclusão"
        description={`Tem certeza que deseja excluir ${itemToDelete?.title}? Esta ação não pode ser desfeita.`}
        onConfirm={handleConfirmDelete}
      />
    </div>
  );
}

export default App;
