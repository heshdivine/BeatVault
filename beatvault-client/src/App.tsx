import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import HomePage from './pages/HomePage'; // Ensure path is correct
import LoginPage from './pages/LoginPage'; // Ensure path is correct
import { Music, UploadCloud } from 'lucide-react';
import AuctionPage from './pages/AuctionPage';
import UploadPage from './pages/UploadPage';
import CheckoutPage from './pages/CheckoutPage';
import SuccessPage from './pages/SuccessPage';

function App() {
  return (
    <BrowserRouter>
      {/* Navigation Bar */}
      <nav className="bg-black/50 backdrop-blur-md border-b border-gray-800 text-white p-4 fixed w-full top-0 z-50">
        <div className="container mx-auto flex justify-between items-center">
          <Link to="/" className="text-2xl font-bold flex items-center gap-2 text-purple-400">
            <Music size={24} /> BeatVault
          </Link>
          <div className="flex gap-4">
            <Link to="/" className="hover:text-purple-400 transition">Store</Link>
            <Link to="/upload" className="hover:text-purple-400 transition flex items-center gap-1">
              <UploadCloud size={18} /> Upload
            </Link>
            <Link to="/login" className="bg-white text-black px-4 py-1 rounded hover:bg-gray-200 transition font-medium">
              Login
            </Link>
          </div>
        </div>


      </nav>

      {/* Main Content Area */}
      <div className="pt-20">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/auction/:id" element={<AuctionPage />} />
          <Route path="/upload" element={<UploadPage />} />
          <Route path="/checkout/:beatId" element={<CheckoutPage />} />
          <Route path="/success" element={<SuccessPage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;