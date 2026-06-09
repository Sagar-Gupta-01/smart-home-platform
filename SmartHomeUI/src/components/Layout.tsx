import Sidebar from "./Sidebar";
import Header from "./Header";

interface Props {
  children: React.ReactNode;
  title?: string;
}

function Layout({ children, title }: Props) {
  return (
    <div className="flex h-screen bg-gray-100 text-gray-800">

      {/* Sidebar */}
      <Sidebar />

      {/* Main */}
      <div className="flex-1 flex flex-col">

        {/* Header */}
        <Header title={title} />

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-8">
          {children}
        </div>

      </div>
    </div>
  );
}

export default Layout;
