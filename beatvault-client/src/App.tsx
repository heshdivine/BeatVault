import { BrowserRouter, Routes, Route, Link, Navigate } from 'react-router-dom';
import { UserProvider, useUser } from './context/UserContext';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import SignupPage from './pages/SignupPage';
import { Music, UploadCloud, User as UserIcon, LogOut, ChevronDown } from 'lucide-react';
import AuctionPage from './pages/AuctionPage';
import UploadPage from './pages/UploadPage';
import CheckoutPage from './pages/CheckoutPage';
import SuccessPage from './pages/SuccessPage';
import { useState, useRef, useEffect } from 'react';

function Navigation() {
  const { user, logout, isAuthenticated, isProducer } = useUser();
  const [showUserMenu, setShowUserMenu] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  // Close menu when clicking outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setShowUserMenu(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <nav className="bg-black/50 backdrop-blur-md border-b border-gray-800 text-white p-4 fixed w-full top-0 z-50">
      <div className="container mx-auto flex justify-between items-center">
        <Link to="/" className="text-2xl font-bold flex items-center gap-2 text-purple-400 hover:text-purple-300 transition">
          <Music size={28} /> BeatVault
        </Link>

        <div className="flex items-center gap-6">
          {/* Navigation Links */}
          <div className="flex gap-6">
            <Link to="/" className="hover:text-purple-400 transition font-medium">
              Store
            </Link>

            {/* Upload - Only for Producers */}
            {isProducer && (
              <Link to="/upload" className="hover:text-purple-400 transition flex items-center gap-1 font-medium">
                <UploadCloud size={18} /> Upload
              </Link>
            )}
          </div>

          {/* Auth Section */}
          {isAuthenticated ? (
            <div className="relative" ref={menuRef}>
              <button
                onClick={() => setShowUserMenu(!showUserMenu)}
                className="flex items-center gap-2 bg-gray-800 hover:bg-gray-700 px-4 py-2 rounded-lg transition border border-gray-700"
              >
                <UserIcon size={18} />
                <span className="font-medium">{user?.username}</span>
                <ChevronDown size={16} className={`transition-transform ${showUserMenu ? 'rotate-180' : ''}`} />
              </button>

              {/* Dropdown Menu */}
              {showUserMenu && (
                <div className="absolute right-0 mt-2 w-56 bg-gray-800 border border-gray-700 rounded-lg shadow-2xl overflow-hidden">
                  <div className="px-4 py-3 border-b border-gray-700">
                    <p className="text-sm text-gray-400">Signed in as</p>
                    <p className="font-medium truncate">{user?.username}</p>
                    <p className="text-xs text-purple-400 mt-1">{user?.role}</p>
                  </div>
                  <button
                    onClick={() => {
                      logout();
                      setShowUserMenu(false);
                    }}
                    className="w-full px-4 py-3 text-left hover:bg-gray-700 transition flex items-center gap-2 text-red-400"
                  >
                    <LogOut size={16} />
                    Logout
                  </button>
                </div>
              )}
            </div>
          ) : (
            <div className="flex gap-3">
              <Link
                to="/login"
                className="px-4 py-2 rounded-lg hover:bg-gray-800 transition font-medium border border-gray-700"
              >
                Login
              </Link>
              <Link
                to="/signup"
                className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg transition font-medium shadow-lg shadow-purple-500/20"
              >
                Sign Up
              </Link>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}

// Protected Route Component
function ProtectedRoute({ children, requireProducer = false }: { children: React.ReactNode; requireProducer?: boolean }) {
  const { isAuthenticated, isProducer, loading } = useUser();

  // Show nothing while loading to prevent flash of wrong content
  if (loading) {
    return (
      <div className="min-h-screen bg-gray-900 flex items-center justify-center">
        <div className="text-purple-400 text-xl">Loading...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requireProducer && !isProducer) {
    return (
      <div className="min-h-screen bg-gray-900 text-white flex items-center justify-center p-6">
        <div className="bg-gray-800 p-8 rounded-2xl border border-gray-700 text-center max-w-md">
          <h2 className="text-2xl font-bold text-red-400 mb-4">Access Denied</h2>
          <p className="text-gray-300 mb-6">
            This feature is only available for Producers. You are currently signed in as an Artist.
          </p>
          <Link to="/" className="bg-purple-600 hover:bg-purple-700 px-6 py-3 rounded-lg font-medium inline-block transition">
            Back to Store
          </Link>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}

function AppContent() {
  return (
    <>
      <Navigation />

      {/* Main Content Area */}
      <div className="pt-20">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/signup" element={<SignupPage />} />
          <Route path="/auction/:id" element={<AuctionPage />} />
          <Route
            path="/upload"
            element={
              <ProtectedRoute requireProducer>
                <UploadPage />
              </ProtectedRoute>
            }
          />
          <Route path="/checkout/:beatId" element={<CheckoutPage />} />
          <Route path="/success" element={<SuccessPage />} />
        </Routes>
      </div>
    </>
  );
}

function App() {
  return (
    <BrowserRouter>
      <UserProvider>
        <AppContent />
      </UserProvider>
    </BrowserRouter>
  );
}

export default App;